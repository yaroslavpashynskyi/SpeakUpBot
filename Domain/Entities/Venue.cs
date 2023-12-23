using Domain.Common;

namespace Domain.Entities;

public class Venue : BaseEntity<Guid>
{
    public required string Name { get; set; }
    public required string City { get; set; }
    public required Uri LocationUrl { get; set; }
    public required Uri InstagramUrl { get; set; }
    public ICollection<Speaking> Speakings { get; private set; } = new List<Speaking>();
}
