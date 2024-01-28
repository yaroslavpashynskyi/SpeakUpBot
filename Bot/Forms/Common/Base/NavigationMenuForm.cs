using Bot.Forms.Admin;
using Bot.Forms.Admin.SpeakingMenu;
using Bot.Forms.Admin.VenueMenu;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.Base;

public class NavigationMenuForm : AutoCleanForm
{
    protected ButtonGrid MenuButtonGrid;
    public List<ButtonBase> MainButtons = new();
    public string MenuTitle = "Меню";
    public bool ShowBackButton = false;

    public NavigationMenuForm()
    {
        MenuButtonGrid = new ButtonGrid { KeyboardType = EKeyboardType.ReplyKeyboard };

        Init += Menu_Init;
    }

    private Task Menu_Init(object sender, InitEventArgs e)
    {
        MenuButtonGrid.Title = MenuTitle;
        MenuButtonGrid.ResizeKeyboard = true;
        if (ShowBackButton)
            MenuButtonGrid.HeadLayoutButtonRow = new List<ButtonBase>
            {
                new("◀️Назад", Device.PreviousForm.GetType().ToString())
            };

        var bf = new ButtonForm();
        bf.AddSplitted(MainButtons);
        MenuButtonGrid.DataSource.ButtonForm = bf;

        MenuButtonGrid.ButtonClicked += Menu_ButtonClicked;

        AddControl(MenuButtonGrid);
        return Task.CompletedTask;
    }

    protected virtual async Task Menu_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (e.Button == null)
            return;

        Type? target = Type.GetType(e.Button.Value);
        if (target != null && target.IsSubclassOf(typeof(FormBase)))
        {
            await this.NavigateTo(target);
            return;
        }
        await HandleOtherButtons(e.Button);
    }

    protected virtual Task HandleOtherButtons(ButtonBase button)
    {
        return Task.CompletedTask;
    }
}
