using Application.Common.Interfaces;
using Application.Speakings.Queries.GetAllSpeakings;

using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Speakings.Queries.GetUserUnregisteredSpeakings;

public class GetUserUnregisteredSpeakings : IRequest<List<Speaking>>
{
    public long UserTelegramId { get; set; }
}

public class GetUserUnregisteredSpeakingsHandler
    : IRequestHandler<GetUserUnregisteredSpeakings, List<Speaking>>
{
    private readonly IApplicationDbContext _context;

    public GetUserUnregisteredSpeakingsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Speaking>> Handle(
        GetUserUnregisteredSpeakings request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Speakings
            .Include(s => s.Venue)
            .Include(s => s.Photos)
            .Where(s => !s.Users.Any(u => u.TelegramId == request.UserTelegramId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
