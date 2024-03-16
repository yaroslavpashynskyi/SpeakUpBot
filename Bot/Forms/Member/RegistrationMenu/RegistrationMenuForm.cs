using Bot.Forms.Common.Base;

using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Member.RegistrationMenu;

public class RegistrationMenuForm : NavigationMenuForm
{
    public RegistrationMenuForm()
    {
        MenuTitle = "Меню записів📖";

        AddBackButton<MemberMenuForm>();
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Записатись на івент🖋", typeof(CreateRegistrationForm).ToString()),
                new ButtonBase(
                    "Переглянути мої записи📖",
                    typeof(MemberRegistrationListForm).ToString()
                ),
            }
        );
        DeleteMode = EDeleteMode.OnLeavingForm;
    }
}
