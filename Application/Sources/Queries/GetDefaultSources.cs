using Application.Common.Interfaces;

using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Sources.Queries;

public class GetDefaultSourcesCommand : IRequest<List<Source>> { }

public class GetDefaultSourcesHandler : IRequestHandler<GetDefaultSourcesCommand, List<Source>>
{
    private readonly IApplicationDbContext _context;

    public GetDefaultSourcesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    async Task<List<Source>> IRequestHandler<GetDefaultSourcesCommand, List<Source>>.Handle(
        GetDefaultSourcesCommand request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Sources.AsNoTracking().Where(s => !s.IsCustom).ToListAsync();
    }
}
