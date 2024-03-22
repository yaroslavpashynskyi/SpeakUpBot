using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity<Guid>, IOrderable
{
    public long TelegramId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Role Role { get; set; } = Role.Member;
    public required string PhoneNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public EnglishLevel EnglishLevel { get; set; }
    public Guid SourceId { get; set; }
    public Source Source { get; set; } = null!;
    public bool TransferTicket { get; set; } = false;
    public ICollection<Registration> Registrations { get; private set; } = new List<Registration>();
    public ICollection<Speaking> Speakings { get; private set; } = new List<Speaking>();

    public object? GetOrderKey() => CreatedAt;
}
