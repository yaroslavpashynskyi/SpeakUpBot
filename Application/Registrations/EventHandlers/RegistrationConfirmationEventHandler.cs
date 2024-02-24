using Application.Common.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.Registrations.EventHandlers;

public class RegistrationConfirmationEventHandler
    : INotificationHandler<RegistrationConfirmationEvent>
{
    private readonly INotificationSender _notificationSender;

    public RegistrationConfirmationEventHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task Handle(
        RegistrationConfirmationEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _notificationSender.SendToGroup(
            $"Користувач <b>{notification.User.LastName} {notification.User.FirstName}</b> "
                + $"підтвердив оплату на {notification.Speaking.Title}."
        );
    }
}
