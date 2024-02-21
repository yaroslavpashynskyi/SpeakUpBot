using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Commands.ForceModifyStatus;

public class ForceModifyStatusCommand : IRequest<Result<Unit, Error>>
{
    public Guid RegistrationId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

public class ForceModifyStatusCommandHandler
    : IRequestHandler<ForceModifyStatusCommand, Result<Unit, Error>>
{
    private readonly IApplicationDbContext _context;

    public ForceModifyStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit, Error>> Handle(
        ForceModifyStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        var registration = await _context.Registrations
            .Include(r => r.Speaking)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == request.RegistrationId);

        if (registration == null)
            return NotFoundErrors<Registration>.EntityNotFound;

        registration.PaymentStatus = request.PaymentStatus;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
