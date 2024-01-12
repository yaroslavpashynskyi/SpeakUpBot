using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Venues.Queries;

public class GetAllVenues : IRequest<List<Venue>> { }

public class GetAllVenuesHandler : IRequestHandler<GetAllVenues, List<Venue>>
{
    private readonly IApplicationDbContext _context;

    public GetAllVenuesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    async Task<List<Venue>> IRequestHandler<GetAllVenues, List<Venue>>.Handle(
        GetAllVenues request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Venues.AsNoTracking().ToListAsync();
    }
}
