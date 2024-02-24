using Application.Common.Interfaces;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Infrastructure.Services;

public class NotificationSender : INotificationSender
{
    private readonly TelegramBotClient _botClient;
    private readonly IConfiguration _config;

    public NotificationSender(TelegramBotClient botClient, IConfiguration config)
    {
        _botClient = botClient;
        _config = config;
    }

    public async Task SendToGroup(string message)
    {
        var groupId = long.Parse(_config["BotConfiguration:NotificationGroupId"]!);
        await _botClient.SendTextMessageAsync(groupId, message, parseMode: ParseMode.Html);
    }

    public async Task SendToUser(string message, long telegramUserId)
    {
        await _botClient.SendTextMessageAsync(telegramUserId, message, parseMode: ParseMode.Html);
    }
}
