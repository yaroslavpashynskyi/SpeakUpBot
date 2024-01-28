using Bot.Extensions;
using Bot.Forms.Admin.SpeakingMenu;
using Bot.Forms.Admin.VenueMenu;
using Bot.Forms.Common.Base;

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
                new ButtonBase("Майбутні спікінги💬", typeof(FutureSpeakingsForm).ToString()),
                new ButtonBase("Мої записи📖", typeof(SpeakingMenuForm).ToString()),
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
