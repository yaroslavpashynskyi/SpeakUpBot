using Bot.Forms.Admin.VenueMenu;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin;

public class AdminMenuForm : AutoCleanForm
{
    private readonly ButtonGrid _mButtons;
    private readonly IReadOnlyList<Type> _allowedForms = new List<Type> { typeof(VenueMenuForm) };

    public AdminMenuForm()
    {
        DeleteMode = EDeleteMode.OnLeavingForm;

        _mButtons = new ButtonGrid { KeyboardType = EKeyboardType.ReplyKeyboard };
        _mButtons.Title = "Головне Меню Адміністратора";
        _mButtons.ResizeKeyboard = true;
        Init += MenuForm_Init;
    }

    private Task MenuForm_Init(object sender, InitEventArgs e)
    {
        var bf = new ButtonForm();

        bf.AddButtonRow(new ButtonBase("Місця проведення📍", typeof(VenueMenuForm).ToString()));

        _mButtons.DataSource.ButtonForm = bf;

        _mButtons.ButtonClicked += Menu_ButtonClicked;

        AddControl(_mButtons);
        return Task.CompletedTask;
    }

    private async Task Menu_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        Type? target = Type.GetType(e.Button.Value);
        if (e.Button == null || target == null)
        {
            return;
        }

        if (_allowedForms.Contains(target))
        {
            await this.NavigateTo(target);
        }
    }
}
