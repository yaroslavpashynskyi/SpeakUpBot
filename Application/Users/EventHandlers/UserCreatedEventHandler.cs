using Application.Common.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.Users.EventHandlers;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly INotificationSender _notificationSender;

    public UserCreatedEventHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _notificationSender.SendToGroup(
            $"Зареєстрований новий користувач: <b>{notification.User.FirstName} {notification.User.LastName}</b>"
        );
    }
}
