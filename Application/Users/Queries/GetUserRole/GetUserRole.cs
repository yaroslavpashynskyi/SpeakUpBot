using Application.Common.Interfaces;

using Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserRole;

public record GetUserRoleQuery(long TelegramId) : IRequest<Role>;

public class GetUserRoleQueryHandler : IRequestHandler<GetUserRoleQuery, Role>
{
    private readonly IApplicationDbContext _context;

    public GetUserRoleQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TelegramId == request.TelegramId)
            .Select(u => u.Role)
            .FirstOrDefaultAsync();
    }
}
