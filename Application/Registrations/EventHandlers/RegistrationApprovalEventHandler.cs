using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Events;
using MediatR;

namespace Application.Registrations.EventHandlers;

public class RegistrationApprovalEventHandler : INotificationHandler<RegistrationApprovalEvent>
{
    private readonly INotificationSender _notificationSender;

    public RegistrationApprovalEventHandler(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task Handle(
        RegistrationApprovalEvent notification,
        CancellationToken cancellationToken
    )
    {
        var eventTime = notification
            .Speaking.TimeOfEvent.ToLocalTime()
            .ToString("HH:mm dd MMMM", new System.Globalization.CultureInfo("uk-UA"));
        var venue = notification.Speaking.Venue;

        var message =
            $"Вітаємо, <b>{notification.User.FirstName}</b>!\n"
            + $"Вашу оплату на {notification.Speaking.GetName()} підтверджено організатором!\n"
            + $"Чекаємо вас об {eventTime} в закладі <a href=\"{venue.LocationUrl}\">{venue.Name}</a>.";

        await _notificationSender.SendToUser(message, notification.User.TelegramId);
    }
}
