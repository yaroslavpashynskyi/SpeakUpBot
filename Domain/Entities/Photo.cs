using Domain.Common;

namespace Domain.Entities;

public class Photo : BaseEntity<Guid>
{
    public int OrdinalNumber { get; set; }
    public required string FileId { get; set; }
    public Guid SpeakingId { get; set; }
    public Speaking Speaking { get; set; } = null!;
}
