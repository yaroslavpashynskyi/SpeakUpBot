using Bot.Forms.Common.Base;

using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Member.RegistrationMenu;

public class RegistrationMenuForm : NavigationMenuForm
{
    public RegistrationMenuForm()
    {
        MenuTitle = "Меню записів📖";
        ShowBackButton = true;
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase(
                    "Записатись на спікінг🖋",
                    typeof(CreateRegistrationForm).ToString()
                ),
                new ButtonBase("Переглянути мої записи📖", typeof(MemberRegistrationList).ToString()),
            }
        );
        DeleteMode = EDeleteMode.OnLeavingForm;
    }
}
