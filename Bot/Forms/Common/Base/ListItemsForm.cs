using Domain.Common;

using MediatR;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.Base;

public class ListItemsForm<T> : AutoCleanForm
    where T : BaseEntity<Guid>
{
    protected readonly ButtonGrid _mButtons;
    protected IMediator _mediator = null!;
    protected List<T> _entities = new();
    protected Func<T, bool> _filter = p => true;

    protected IRequest<List<T>> _request = null!;
    protected string _listTitle = "Список";

    public ListItemsForm()
    {
        DeleteMode = EDeleteMode.OnLeavingForm;

        _mButtons = new ButtonGrid
        {
            KeyboardType = EKeyboardType.ReplyKeyboard,
            EnablePaging = true,
            HeadLayoutButtonRow = new List<ButtonBase> { new("◀️Назад", "back") }
        };

        Init += ListForm_Init;
    }

    protected virtual async Task ListForm_Init(object sender, InitEventArgs e)
    {
        _mButtons.Title = _listTitle;
        _mButtons.ResizeKeyboard = true;
        _entities = (await _mediator.Send(_request)).Where(_filter).ToList();

        var bf = new ButtonForm();

        foreach (var entity in _entities)
        {
            bf.AddButtonRow(GetButtonName(entity), entity.Id.ToString());
        }
        _mButtons.DataSource.ButtonForm = bf;

        _mButtons.ButtonClicked += List_ButtonClicked;

        AddControl(_mButtons);
    }

    protected virtual string GetButtonName(T entity)
    {
        return nameof(T);
    }

    protected virtual async Task List_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (e.Button == null)
        {
            return;
        }

        if (e.Button.Value == "back")
        {
            await this.NavigateTo(Device.PreviousForm.GetType());
            return;
        }

        if (!Guid.TryParse(e.Button.Value, out Guid selectedId))
            return;

        var entity = _entities.FirstOrDefault(v => v.Id == selectedId);

        if (entity == null)
        {
            await Device.Send("Помилка! Такої сутності не знайдено!");
            return;
        }

        await HandleEntity(entity);
    }

    protected virtual Task HandleEntity(T entity)
    {
        throw new NotImplementedException();
    }
}
