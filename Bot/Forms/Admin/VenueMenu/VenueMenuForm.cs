using Bot.Forms.Common.Base;

using TelegramBotBase.Form;

namespace Bot.Forms.Admin.VenueMenu;

public class VenueMenuForm : NavigationMenuForm
{
    public VenueMenuForm()
    {
        MenuTitle = "Меню місць проведення";

        AddBackButton<AdminMenuForm>();
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Створити місце➕", typeof(CreateVenueForm).ToString()),
                new ButtonBase("Список місць🗒", typeof(VenueListForm).ToString())
            }
        );
    }
}
