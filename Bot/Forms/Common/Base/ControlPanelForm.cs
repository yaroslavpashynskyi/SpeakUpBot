using Bot.Extensions;

using Domain.Common;

using Telegram.Bot.Types.Enums;

using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.Base;

public class ControlPanelForm<T> : ListItemsForm<T>
    where T : BaseEntity<Guid>
{
    protected bool _controlMode = false;
    protected T? _selectedEntity;
    protected readonly ButtonForm _controlModeForm = new ButtonForm();

    protected ButtonBase? _backToMenuButton;
    protected ActionButton[] _controlButtons = Array.Empty<ActionButton>();

    public ControlPanelForm()
    {
        _mButtons.KeyboardType = EKeyboardType.ReplyKeyboard;
        DeleteMode = EDeleteMode.OnEveryCall;
        _mButtons.MessageParseMode = ParseMode.Html;
        _mButtons.ButtonClicked += OnControlMode_ButtonClicked;
        _mButtons.DeletePreviousMessage = false;

        _controlModeForm.AddButtonRow(new ButtonBase("◀️Повернутись до списку", "backToList"));
    }

    protected override Task List_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (e.Button?.Value == "back" && DeleteMode == EDeleteMode.OnEveryCall)
        {
            MessageCleanup();
        }
        return _controlMode ? Task.CompletedTask : base.List_ButtonClicked(sender, e);
    }

    public override Task PreLoad(MessageResult message)
    {
        if (_controlMode && message.IsAction)
            return base.PreLoad(message);

        return Task.CompletedTask;
    }

    private async Task OnControlMode_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (_controlMode)
        {
            if (e.Button == null)
                return;

            if (e.Button.Value == "backToList")
            {
                await RenderList();
                return;
            }

            if (_selectedEntity == null)
                return;

            var selectedButton = _controlButtons.FirstOrDefault(b => b.IsEqual(e.Button));
            if (selectedButton != null)
            {
                await selectedButton.InvokeAction();
                await RenderList();
            }
        }
    }

    private async Task RenderList()
    {
        _controlMode = false;
        _selectedEntity = null;
        _mButtons.EnablePaging = true;

        var bf = new ButtonForm();
        await SetEntities();
        foreach (var entity in _entities)
            bf.AddButtonRow(GetButtonName(entity), entity.Id.ToString());

        _mButtons.DataSource.ButtonForm = bf;
        _mButtons.HeadLayoutButtonRow = new(_backToMenuButton);
        _mButtons.Title = _listTitle;
        _mButtons.KeyboardType = EKeyboardType.ReplyKeyboard;
        _mButtons.Updated();
    }

    protected override Task HandleEntity(T entity)
    {
        RenderControlPanel(entity);
        return Task.CompletedTask;
    }

    protected void RenderControlPanel(T entity)
    {
        _controlMode = true;
        _selectedEntity = entity;
        _mButtons.EnablePaging = false;
        _controlButtons = SetControlButtons(entity);
        ActionButton[] controlButtons = _controlButtons;

        var bf = _controlModeForm.Duplicate();
        bf.AddSplitted(controlButtons, 1);
        _mButtons.DataSource.ButtonForm = bf;

        _backToMenuButton = _mButtons.HeadLayoutButtonRow.ToList().First();
        _mButtons.HeadLayoutButtonRow = null;
        _mButtons.Title = GetControlTitle(entity);
        _mButtons.KeyboardType = EKeyboardType.InlineKeyBoard;
        _mButtons.Updated();
    }

    protected virtual ActionButton[] SetControlButtons(T entity)
    {
        return Array.Empty<ActionButton>();
    }

    protected virtual string GetControlTitle(T entity)
    {
        return typeof(T).Name;
    }
}
