using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Registration : BaseEntity<Guid>, IOrderable
{
    public Guid SpeakingId { get; set; }
    public Speaking Speaking { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public object? GetOrderKey() => RegistrationDate;
}
