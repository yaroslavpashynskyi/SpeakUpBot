using Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;
using Bot.Forms.Common.Base;

using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingMenuForm : NavigationMenuForm
{
    public SpeakingMenuForm()
    {
        MenuTitle = "Меню спікінгів";

        AddBackButton<AdminMenuForm>();
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Створити спікінг➕", typeof(StartCreatingSpeakingForm).ToString()),
                new ButtonBase("Список спікінгів🗒", typeof(SpeakingListForm).ToString())
            }
        );
    }
}
