using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class RegistrationCreatedEvent : BaseEvent
{
    public RegistrationCreatedEvent(Registration registration, User user, Speaking speaking)
    {
        Registration = registration;
        User = user;
        Speaking = speaking;
    }

    public Registration Registration { get; }
    public User User { get; }
    public Speaking Speaking { get; }
}
