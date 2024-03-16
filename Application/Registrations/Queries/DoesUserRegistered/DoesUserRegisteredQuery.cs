using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Queries.DoesUserRegistered;

public class DoesUserRegisteredQuery : IRequest<Result<bool, Error>>
{
    public long TelegramUserId { get; init; }
    public Guid SpeakingId { get; init; }
}

public class DoesUserRegisteredQueryHandler
    : IRequestHandler<DoesUserRegisteredQuery, Result<bool, Error>>
{
    private readonly IApplicationDbContext _context;

    public DoesUserRegisteredQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool, Error>> Handle(
        DoesUserRegisteredQuery request,
        CancellationToken cancellationToken
    )
    {
        var speaking = await _context.Speakings
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == request.SpeakingId);
        if (speaking == null)
            return NotFoundErrors<Speaking>.EntityNotFound;

        return speaking.Users.Any(u => u.TelegramId == request.TelegramUserId);
    }
}
