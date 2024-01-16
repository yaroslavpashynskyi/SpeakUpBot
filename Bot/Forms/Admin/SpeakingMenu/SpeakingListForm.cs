using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingListForm : AutoCleanForm
{
    public override async Task Load(MessageResult message)
    {
        await Device.Send(message.MessageText);
    }
}
