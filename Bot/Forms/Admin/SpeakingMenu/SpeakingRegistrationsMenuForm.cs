using MediatR;

using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingRegistrationsMenuForm : AutoCleanForm
{
    public override Task Load(MessageResult message)
    {
        return base.Load(message);
    }
}
