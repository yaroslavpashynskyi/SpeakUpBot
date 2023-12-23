using Domain.Common;

namespace Domain.Entities;

public class Speaking : BaseEntity<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string Title { get; set; }
    public string? Intro { get; set; }
    public required string Description { get; set; }
    public ICollection<Photo> Photos { get; private set; } = new List<Photo>();
    public ICollection<Registration> Registrations { get; private set; } = new List<Registration>();
    public ICollection<User> Users { get; private set; } = new List<User>();
    public int Price { get; set; }
    public int Seats { get; set; }
    public DateTime TimeOfEvent { get; set; }
    public int DurationMinutes { get; set; }
    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
}
