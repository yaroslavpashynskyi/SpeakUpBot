using Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;
using Bot.Forms.Common.Base;

using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingMenuForm : NavigationMenuForm
{
    public SpeakingMenuForm()
    {
        MenuTitle = "Меню івентів";

        AddBackButton<AdminMenuForm>();
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Створити івент➕", typeof(StartCreatingSpeakingForm).ToString()),
                new ButtonBase("Список івентів🗒", typeof(SpeakingListForm).ToString())
            }
        );
    }
}
