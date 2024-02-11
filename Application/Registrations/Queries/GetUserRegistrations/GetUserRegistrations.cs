using Application.Common.Interfaces;

using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Queries.GetUserRegistrations;

public class GetUserRegistrations : IRequest<List<Registration>>
{
    public long UserTelegramId { get; set; }
}

public class GetuserRegistrationsHandler : IRequestHandler<GetUserRegistrations, List<Registration>>
{
    private readonly IApplicationDbContext _context;

    public GetuserRegistrationsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Registration>> Handle(
        GetUserRegistrations request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Registrations
            .Include(r => r.Speaking.Photos)
            .Include(r => r.Speaking.Venue)
            .Where(r => r.User.TelegramId == request.UserTelegramId)
            .AsNoTracking()
            .ToListAsync();
    }
}
