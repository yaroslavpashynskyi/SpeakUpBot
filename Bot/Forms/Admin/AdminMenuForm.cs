using Bot.Extensions;
using Bot.Forms.Admin.SpeakingMenu;
using Bot.Forms.Admin.VenueMenu;
using Bot.Forms.Common.Base;

using Telegram.Bot.Types.Enums;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin;

public class AdminMenuForm : NavigationMenuForm
{
    public AdminMenuForm()
    {
        MenuTitle = "Головне меню адміністратора";
        MainButtons.AddRange(
            new[]
            {
                new ButtonBase("Місця проведення📍", typeof(VenueMenuForm).ToString()),
                new ButtonBase("Спікінги🗣", typeof(SpeakingMenuForm).ToString())
            }
        );
    }
}
