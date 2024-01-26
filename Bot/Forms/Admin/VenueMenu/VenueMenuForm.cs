using Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;
using Bot.Forms.Admin.SpeakingMenu;
using Bot.Forms.Common.Base;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.VenueMenu;

public class VenueMenuForm : NavigationMenuForm
{
    public VenueMenuForm()
    {
        MenuTitle = "Меню місць проведення";
        TopButton.Text = "◀️Назад";
        TopButton.Value = typeof(AdminMenuForm).ToString();
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Створити місце➕", typeof(CreateVenueForm).ToString()),
                new ButtonBase("Список місць🗒", typeof(VenueListForm).ToString())
            }
        );
    }
}
