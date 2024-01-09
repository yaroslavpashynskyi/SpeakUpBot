using Application.Common.Interfaces;

using Domain.Entities;
using Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<Guid>
{
    public long TelegramId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public EnglishLevel EnglishLevel { get; set; }
    public Source Source { get; set; } = null!;
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
        var source = await _context.Sources.FirstOrDefaultAsync(s => s.Id == request.Source.Id);

        if (source == null)
        {
            return Guid.Empty;
        }

        var user = new User
        {
            TelegramId = request.TelegramId,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EnglishLevel = request.EnglishLevel,
            Source = source
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
