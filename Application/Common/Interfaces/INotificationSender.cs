namespace Application.Common.Interfaces;

public interface INotificationSender
{
    public Task SendToGroup(string message);
    public Task SendToUser(string message, long telegramUserId);
}
