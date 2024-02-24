using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class RegistrationApprovalEvent : BaseEvent
{
    public RegistrationApprovalEvent(User user, Speaking speaking)
    {
        User = user;
        Speaking = speaking;
    }

    public User User { get; }
    public Speaking Speaking { get; }
}
