using Domain.Common;

namespace Domain.Entities;

public class Source : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public bool IsCustom { get; set; } = false;
    public ICollection<User> Users { get; private set; } = new List<User>();
}
