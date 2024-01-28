using Application.Common.Interfaces;

using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Speakings.Queries.GetAllSpeakings;

public class GetAllSpeakingsWithVenue : IRequest<List<Speaking>> { }

public class GetAllSpeakingsWithVenueHandler
    : IRequestHandler<GetAllSpeakingsWithVenue, List<Speaking>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSpeakingsWithVenueHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Speaking>> Handle(
        GetAllSpeakingsWithVenue request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Speakings
            .Include(s => s.Venue)
            .Include(s => s.Photos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
