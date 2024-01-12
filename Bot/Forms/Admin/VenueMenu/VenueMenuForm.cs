using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.VenueMenu;

public class VenueMenuForm : AutoCleanForm
{
    private readonly ButtonGrid _mButtons;
    private readonly IReadOnlyList<Type> _allowedForms = new List<Type>
    {
        typeof(CreateVenueForm),
        typeof(AdminMenuForm),
        typeof(VenueListForm)
    };

    public VenueMenuForm()
    {
        DeleteMode = EDeleteMode.OnLeavingForm;

        _mButtons = new ButtonGrid
        {
            KeyboardType = EKeyboardType.ReplyKeyboard,
            HeadLayoutButtonRow = new List<ButtonBase>
            {
                new("◀️Назад", typeof(AdminMenuForm).ToString())
            }
        };
        _mButtons.Title = "Меню місць проведення";
        _mButtons.ResizeKeyboard = true;
        Init += MenuForm_Init;
    }

    private Task MenuForm_Init(object sender, InitEventArgs e)
    {
        var bf = new ButtonForm();

        bf.AddButtonRow(
            new ButtonBase("Створити місце➕", typeof(CreateVenueForm).ToString()),
            new ButtonBase("Список місць🗒", typeof(VenueListForm).ToString())
        );

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
