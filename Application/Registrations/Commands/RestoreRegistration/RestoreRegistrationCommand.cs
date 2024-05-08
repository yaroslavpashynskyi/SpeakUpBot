using System.Collections.Immutable;

using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Commands.RestoreRegistration;

public class RestoreRegistrationCommand : IRequest<Result<PaymentStatus, Error>>
{
    public Registration Registration { get; set; } = null!;
}

public class RestoreRegistrationCommandHandler
    : IRequestHandler<RestoreRegistrationCommand, Result<PaymentStatus, Error>>
{
    private readonly IApplicationDbContext _context;

    public RestoreRegistrationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaymentStatus, Error>> Handle(
        RestoreRegistrationCommand request,
        CancellationToken cancellationToken
    )
    {
        var registration = await _context.Registrations
            .Include(r => r.Speaking)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == request.Registration.Id);
        if (registration == null)
            return NotFoundErrors<Registration>.EntityNotFound;

        if (
            registration.RegistrationDate
            > registration.Speaking.TimeOfEvent + TimeSpan.FromHours(1)
        )
            return RegistrationErrors.RegistrationTimeout;

        var speaking = await _context.Speakings
            .Include(s => s.Registrations)
            .FirstOrDefaultAsync(s => s.Id == request.Registration.SpeakingId);

        if (speaking == null)
            return NotFoundErrors<Speaking>.EntityNotFound;

        switch (registration.PaymentStatus)
        {
            case PaymentStatus.Cancelled:
                HandleCancelled(registration, speaking);
                break;
            case PaymentStatus.InReserve:
                if (!HandleInReserve(registration, speaking))
                    return RegistrationErrors.RegistrationInReserve;
                break;
            default:
                return RegistrationErrors.RegistrationAlreadyActive;
        }

        registration.AddDomainEvent(
            new RegistrationCreatedEvent(registration, registration.User, registration.Speaking)
        );
        await _context.SaveChangesAsync(cancellationToken);
        return registration.PaymentStatus;
    }

    private void HandleCancelled(Registration registration, Speaking speaking)
    {
        var inReserve = speaking.Registrations.Count(
            r => r.PaymentStatus == PaymentStatus.InReserve
        );
        if (inReserve >= speaking.AvailableSeats)
        {
            registration.PaymentStatus = PaymentStatus.InReserve;
        }
        else if (registration.User.TransferTicket)
        {
            registration.PaymentStatus = PaymentStatus.PaidByTransferTicket;
            registration.User.TransferTicket = false;
        }
        else
        {
            registration.PaymentStatus = PaymentStatus.Pending;
        }

        registration.RegistrationDate = DateTime.UtcNow;
    }

    private bool HandleInReserve(Registration registration, Speaking speaking)
    {
        ImmutableList<Guid> inReserve = speaking.Registrations
            .Where(
                r =>
                    r.SpeakingId == registration.SpeakingId
                    && r.PaymentStatus == PaymentStatus.InReserve
            )
            .OrderBy(r => r.RegistrationDate)
            .Select(r => r.Id)
            .ToImmutableList();

        if (inReserve.IndexOf(registration.Id) < registration.Speaking.AvailableSeats)
        {
            registration.PaymentStatus = PaymentStatus.Pending;
            registration.RegistrationDate = DateTime.UtcNow;
            return true;
        }

        return false;
    }
}
