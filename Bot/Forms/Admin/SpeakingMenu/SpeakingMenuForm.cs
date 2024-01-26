using Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;
using Bot.Forms.Admin.VenueMenu;
using Bot.Forms.Common.Base;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingMenuForm : NavigationMenuForm
{
    public SpeakingMenuForm()
    {
        MenuTitle = "Меню спікінгів";
        TopButton.Text = "◀️Назад";
        TopButton.Value = typeof(AdminMenuForm).ToString();
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Створити спікінг➕", typeof(StartCreatingSpeakingForm).ToString()),
                new ButtonBase("Список спікінгів🗒", typeof(SpeakingListForm).ToString())
            }
        );
    }
}
