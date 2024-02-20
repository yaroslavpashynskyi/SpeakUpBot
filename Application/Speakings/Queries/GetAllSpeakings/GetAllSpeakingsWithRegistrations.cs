using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Speakings.Queries.GetAllSpeakings;

public class GetAllSpeakingsWithRegistrations : IRequest<List<Speaking>> { }

public class GetAllSpeakingsWithRegistrationsHandler
    : IRequestHandler<GetAllSpeakingsWithRegistrations, List<Speaking>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSpeakingsWithRegistrationsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Speaking>> Handle(
        GetAllSpeakingsWithRegistrations request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Speakings
            .Include(s => s.Registrations)
            .Include(s => s.Venue)
            .Include(s => s.Photos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
