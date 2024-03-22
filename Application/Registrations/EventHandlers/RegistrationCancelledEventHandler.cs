using Application.Common.Interfaces;

using Domain.Events;

using MediatR;

namespace Application.Registrations.EventHandlers;

public class RegistrationCancelledEventHandler : INotificationHandler<RegistrationCancelledEvent>
{
    private readonly INotificationSender _notificationSender;

    public RegistrationCancelledEventHandler(INotificationSender _notificationSender)
    {
        this._notificationSender = _notificationSender;
    }

    public async Task Handle(
        RegistrationCancelledEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _notificationSender.SendToGroup(
            $"Користувач <b>{notification.User.LastName} {notification.User.FirstName}</b> "
                + $"скасував запис на {notification.Speaking.Title}."
        );
    }
}
