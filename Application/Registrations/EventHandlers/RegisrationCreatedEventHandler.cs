using Application.Common.Interfaces;

using Domain.Events;

using MediatR;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

namespace Application.Registrations.EventHandlers;

public class RegisrationCreatedEventHandler : INotificationHandler<RegistrationCreatedEvent>
{
    private readonly INotificationSender _notificationSender;

    public RegisrationCreatedEventHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task Handle(
        RegistrationCreatedEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _notificationSender.SendToGroup(
            $"Користувач <b>{notification.User.LastName} {notification.User.FirstName}</b> "
                + $"записався на {notification.Speaking.Title}."
                + $" Статус платежу реєстрації: {notification.Registration.PaymentStatus}"
        );
    }
}
