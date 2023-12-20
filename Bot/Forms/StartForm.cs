using Microsoft.Extensions.Logging;

using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace Bot.Forms;

public class StartForm : FormBase
{
    private readonly ILogger<StartForm> _logger;

    public StartForm(ILogger<StartForm> logger)
    {
        _logger = logger;
    }

    public override async Task Load(MessageResult message)
    {
        // simple echo
        _logger.LogInformation(message.MessageText);
        await Device.Send(message.MessageText);
    }
}
