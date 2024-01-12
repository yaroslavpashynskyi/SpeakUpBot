using Application.Common.Interfaces;

using Domain.Entities;

using MediatR;

namespace Application.Venues.Commands.CreateVenue;

public class CreateVenueCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public Uri Location { get; set; } = null!;
    public Uri Instagram { get; set; } = null!;
}

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateVenueCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = new Venue
        {
            Name = request.Name,
            City = request.City,
            LocationUrl = request.Location,
            InstagramUrl = request.Instagram
        };

        _context.Venues.Add(venue);

        await _context.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}
