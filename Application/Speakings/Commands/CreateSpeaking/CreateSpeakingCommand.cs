using System.Linq;

using Application.Common.Interfaces;
using Application.Extensions;

using Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Speakings.Commands.CreateSpeaking;

public class CreateSpeakingCommand : IRequest<Guid>
{
    public string Title { get; set; } = null!;
    public string? Intro { get; set; }
    public string Description { get; set; } = null!;
    public int Price { get; set; }
    public int Seats { get; set; }
    public List<PhotoDto> Photos { get; set; } = null!;
    public DateOnly DateOfEvent { get; set; }
    public TimeOnly TimeOfEvent { get; set; }
    public int DurationMinutes { get; set; }
    public Venue Venue { get; set; } = null!;
}

public class CreateSpeakingCommandHandler : IRequestHandler<CreateSpeakingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSpeakingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateSpeakingCommand request,
        CancellationToken cancellationToken
    )
    {
        var selectedVenue = await _context.Venues.FirstOrDefaultAsync(
            v => v.Id == request.Venue.Id
        );

        if (selectedVenue == null)
        {
            return Guid.Empty;
        }

        var speaking = request.CreateCommandToSpeaking();
        speaking.Venue = selectedVenue;

        _context.Speakings.Add(speaking);

        await _context.SaveChangesAsync(cancellationToken);

        return speaking.Id;
    }
}
