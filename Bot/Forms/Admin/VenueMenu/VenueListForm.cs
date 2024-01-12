using Application.Venues.Queries;

using Bot.Utils;

using Domain.Entities;

using MediatR;

using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.VenueMenu;

public class VenueListForm : AutoCleanForm
{
    private readonly ButtonGrid _mButtons;
    private readonly IMediator _mediator;
    private List<Venue> _venues = new();

    public VenueListForm(IMediator mediator)
    {
        _mediator = mediator;

        DeleteMode = EDeleteMode.OnLeavingForm;

        _mButtons = new ButtonGrid
        {
            KeyboardType = EKeyboardType.ReplyKeyboard,
            EnablePaging = true,
            HeadLayoutButtonRow = new List<ButtonBase> { new("◀️Назад", "back") }
        };
        _mButtons.Title = "Список місць проведення";
        _mButtons.ResizeKeyboard = true;
        Init += ListForm_Init;
    }

    private async Task ListForm_Init(object sender, InitEventArgs e)
    {
        _venues = await _mediator.Send(new GetAllVenues());
        var bf = new ButtonForm();

        foreach (var venue in _venues)
        {
            bf.AddButtonRow(venue.Name, venue.Id.ToString());
        }
        _mButtons.DataSource.ButtonForm = bf;

        _mButtons.ButtonClicked += List_ButtonClicked;

        AddControl(_mButtons);
    }

    private async Task List_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (e.Button == null)
        {
            return;
        }

        if (e.Button.Value == "back")
        {
            await this.NavigateTo<VenueMenuForm>();
            return;
        }

        Guid.TryParse(e.Button.Value, out Guid selectedVenue);
        if (selectedVenue == Guid.Empty)
            return;

        var venue = _venues.Find(v => v.Id == selectedVenue);

        if (venue == null)
        {
            await Device.Send("Помилка! Такого місця проведення не знайдено!");
            return;
        }

        await Device.Send(
            FormatVenue.Format(venue.Name, venue.City, venue.LocationUrl, venue.InstagramUrl),
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
}
