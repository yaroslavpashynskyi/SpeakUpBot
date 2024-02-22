using Application.Common.Interfaces;

using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<User>> { }

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<User>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Source)
            .ToListAsync(cancellationToken);
    }
}
