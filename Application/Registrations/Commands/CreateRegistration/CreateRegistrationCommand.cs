using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Commands.CreateRegistration;

public class CreateRegistrationCommand : IRequest<Result<Registration, Error>>
{
    public Speaking Speaking { get; set; } = null!;
    public long UserTelegramId { get; set; }
}

public class CreateRegistrationCommandHandler
    : IRequestHandler<CreateRegistrationCommand, Result<Registration, Error>>
{
    private readonly IApplicationDbContext _context;

    public CreateRegistrationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Registration, Error>> Handle(
        CreateRegistrationCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _context.Users
            .Include(u => u.Registrations)
            .FirstOrDefaultAsync(u => u.TelegramId == request.UserTelegramId);
        if (user == null)
            return NotFoundErrors<User>.EntityNotFound;

        var speaking = await _context.Speakings.FirstOrDefaultAsync(
            s => s.Id == request.Speaking.Id
        );
        if (speaking == null)
            return NotFoundErrors<Speaking>.EntityNotFound;

        var isRegistrationDuplicate = await _context.Registrations.AnyAsync(
            r => r.Speaking.Id == speaking.Id && r.User.Id == user.Id
        );

        if (isRegistrationDuplicate)
            return RegistrationErrors.RegistrationDuplication;

        var registration = new Registration() { Speaking = speaking, User = user };

        if (registration.RegistrationDate > speaking.TimeOfEvent + TimeSpan.FromHours(1))
            return RegistrationErrors.RegistrationTimeout;

        if (user.TransferTicket)
        {
            user.TransferTicket = false;
            registration.PaymentStatus = PaymentStatus.PaidByTransferTicket;
        }

        await _context.Registrations.AddAsync(registration);

        await _context.SaveChangesAsync(cancellationToken);
        return registration;
    }
}
