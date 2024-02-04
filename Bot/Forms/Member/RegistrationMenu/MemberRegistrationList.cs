using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace Bot.Forms.Member.RegistrationMenu;

public class MemberRegistrationList : AutoCleanForm
{
    public override async Task Load(MessageResult message)
    {
        await Device.Send("У розробці");
    }
}
