using Application.Common.Interfaces;
using Application.Common.Models;

using Domain.Common;
using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Registrations.Queries.GetUserRegistrations;

public class GetSpeakingRegistrations : IRequest<Result<List<Registration>, Error>>
{
    public Guid SpeakingId { get; set; }
}

public class GetSpeakingRegistrationsHandler
    : IRequestHandler<GetSpeakingRegistrations, Result<List<Registration>, Error>>
{
    private readonly IApplicationDbContext _context;

    public GetSpeakingRegistrationsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Registration>, Error>> Handle(
        GetSpeakingRegistrations request,
        CancellationToken cancellationToken
    )
    {
        var speaking = await _context.Speakings.FirstOrDefaultAsync(
            s => request.SpeakingId == s.Id
        );

        if (speaking == null)
            return NotFoundErrors<Speaking>.EntityNotFound;

        return await _context.Registrations
            .Where(r => r.Speaking.Id == speaking.Id)
            .Include(r => r.User)
            .Include(r => r.Speaking)
            .AsNoTracking()
            .ToListAsync();
    }
}
