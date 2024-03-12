using Bot.Extensions;
using Bot.Forms.Common.Base;
using Bot.Forms.Member.RegistrationMenu;

using Telegram.Bot.Types.Enums;

using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Member;

public class MemberMenuForm : NavigationMenuForm
{
    private readonly ButtonBase _contactButton = new("Зв'язок з організатором📞", "contact");

    public MemberMenuForm()
    {
        MenuTitle = "Головне меню🕹";
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Майбутні івенти🚀", typeof(FutureSpeakingsForm).ToString()),
                new ButtonBase("Мої записи📖", typeof(RegistrationMenuForm).ToString()),
                _contactButton
            }
        );
        DeleteMode = EDeleteMode.OnLeavingForm;
    }

    protected override async Task HandleOtherButtons(ButtonBase button)
    {
        if (button.IsEqual(_contactButton))
            await Device.Send(
                "Якщо виникли питання, пишіть організатору 👉 @bogdan_pash",
                parseMode: ParseMode.Html
            );
    }
}
