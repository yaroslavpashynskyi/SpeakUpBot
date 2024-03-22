using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class RegistrationCancelledEvent : BaseEvent
{
    public RegistrationCancelledEvent(User user, Speaking speaking)
    {
        User = user;
        Speaking = speaking;
    }

    public User User { get; }
    public Speaking Speaking { get; }
}
