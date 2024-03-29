﻿using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Commands.CancelRegistration;

public class CancelRegistrationCommand : IRequest<Result<CancelledRegistrationResult, Error>>
{
    public Registration Registration { get; set; } = null!;
}

public class CancelRegistrationCommandHandler
    : IRequestHandler<CancelRegistrationCommand, Result<CancelledRegistrationResult, Error>>
{
    private readonly IApplicationDbContext _context;

    public CancelRegistrationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CancelledRegistrationResult, Error>> Handle(
        CancelRegistrationCommand request,
        CancellationToken cancellationToken
    )
    {
        var registration = await _context.Registrations
            .Include(r => r.Speaking)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == request.Registration.Id);
        if (registration == null)
            return NotFoundErrors<Registration>.EntityNotFound;

        var now = DateTime.UtcNow;
        if (now > registration.Speaking.TimeOfEvent + TimeSpan.FromHours(1))
            return RegistrationErrors.RegistrationTimeout;

        var cancelletionResult = new CancelledRegistrationResult() { TransferTicketGained = false };

        switch (registration.PaymentStatus)
        {
            case PaymentStatus.Pending
            or PaymentStatus.ToBePaidByCash:
                registration.PaymentStatus = PaymentStatus.Cancelled;
                break;
            case PaymentStatus.ToBeApproved:
                return RegistrationErrors.RegistrationNeedToBeApproved;
            case PaymentStatus.PaidByCard
            or PaymentStatus.PaidByTransferTicket:
                if (registration.Speaking.TimeOfEvent + TimeSpan.FromHours(48) > now)
                {
                    registration.User.TransferTicket = true;
                    cancelletionResult.TransferTicketGained = true;
                }

                registration.PaymentStatus = PaymentStatus.Cancelled;
                break;
            case PaymentStatus.Cancelled:
                return RegistrationErrors.RegistrationAlreadyCancelled;
        }

        registration.AddDomainEvent(
            new RegistrationCancelledEvent(registration.User, registration.Speaking)
        );
        await _context.SaveChangesAsync(cancellationToken);

        return cancelletionResult;
    }
}
