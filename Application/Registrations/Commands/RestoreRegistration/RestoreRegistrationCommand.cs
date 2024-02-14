using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;

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

        if (registration.PaymentStatus == PaymentStatus.Cancelled)
        {
            if (registration.User.TransferTicket)
            {
                registration.PaymentStatus = PaymentStatus.PaidByTransferTicket;
                registration.User.TransferTicket = false;
            }
            else
                registration.PaymentStatus = PaymentStatus.Pending;

            registration.RegistrationDate = DateTime.UtcNow;
        }
        else
            return RegistrationErrors.RegistrationNotCancelled;

        await _context.SaveChangesAsync(cancellationToken);
        return registration.PaymentStatus;
    }
}
