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
        await _notificationSender.SendToUser(
            $"Вітаємо, <b>{notification.User.FirstName}</b>!\n"
                + $"Вашу оплату на {notification.Speaking.GetName()} підтверджено організатором!",
            notification.User.TelegramId
        );
    }
}
