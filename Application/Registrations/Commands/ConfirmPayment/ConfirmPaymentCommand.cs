using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Commands.ConfirmPayment;

public class ConfirmPaymentCommand : IRequest<Result<Unit, Error>>
{
    public Registration Registration { get; set; } = null!;
}

public class ConfirmPaymentCommandHandler
    : IRequestHandler<ConfirmPaymentCommand, Result<Unit, Error>>
{
    private readonly IApplicationDbContext _context;

    public ConfirmPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit, Error>> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var registration = await _context.Registrations
            .Include(r => r.Speaking)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == request.Registration.Id);
        if (registration == null)
            return NotFoundErrors<Registration>.EntityNotFound;

        if (registration.PaymentStatus != PaymentStatus.Pending)
            return RegistrationErrors.PaymentNotPending;

        registration.PaymentStatus = PaymentStatus.ToBeApproved;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
