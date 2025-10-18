using Application.Common.Interfaces;

using Domain.Entities;
using Domain.Enums;
using Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<Guid>
{
    public long TelegramId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public EnglishLevel EnglishLevel { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        
        var user = new User
        {
            TelegramId = request.TelegramId,
            PhoneNumber = request.PhoneNumber,
            Name = request.Name,
            EnglishLevel = request.EnglishLevel,
        };

        _context.Users.Add(user);

        user.AddDomainEvent(new UserCreatedEvent(user));
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
