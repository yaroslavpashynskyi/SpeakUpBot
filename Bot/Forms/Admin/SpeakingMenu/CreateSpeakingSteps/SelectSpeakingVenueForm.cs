using Application.Speakings.Commands.CreateSpeaking;
using Application.Venues.Queries;
using Telegram.Bot.Types.Enums;
using Bot.Utils;

using Domain.Entities;

using MediatR;
using TelegramBotBase.Args;
using TelegramBotBase.Controls.Hybrid;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;
using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;

namespace Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;

public class SelectSpeakingVenueForm : AutoCleanForm
{
    private readonly ButtonGrid _venueButtons;
    private readonly IMediator _mediator;
    private CreateSpeakingCommand _speakingData;
    private List<Venue> _venues;

    public SelectSpeakingVenueForm(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _speakingData = new CreateSpeakingCommand();
        _venues = new List<Venue>();

        DeleteMode = EDeleteMode.OnLeavingForm;
        _venueButtons = InitializeButtonGrid();
        Init += InitializeForm;
    }

    private ButtonGrid InitializeButtonGrid()
    {
        var buttonGrid = new ButtonGrid
        {
            Title = "Список місць проведення",
            KeyboardType = EKeyboardType.ReplyKeyboard,
            EnablePaging = true,
            ResizeKeyboard = true
        };
        buttonGrid.ButtonClicked += OnButtonClicked;
        return buttonGrid;
    }

    private async Task InitializeForm(object sender, InitEventArgs e)
    {
        _speakingData = (CreateSpeakingCommand)e.Args[0];
        _venues = await _mediator.Send(new GetAllVenues());
        PopulateVenueButtons();
        AddControl(_venueButtons);
    }

    private void PopulateVenueButtons()
    {
        var buttonForm = new ButtonForm();
        foreach (var venue in _venues)
        {
            buttonForm.AddButtonRow(venue.Name, venue.Id.ToString());
        }
        _venueButtons.DataSource.ButtonForm = buttonForm;
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();
        if (TryGetVenue(message.RawData, out var venue))
        {
            _speakingData.Venue = venue!;
            await this.NavigateTo<PreviewAssembledSpeakingForm>(_speakingData);
        }
        else
        {
            await NotifyVenueNotFoundError();
        }
    }

    private async Task OnButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (e.Button == null)
            return;

        if (TryGetVenue(e.Button.Value, out var venue))
        {
            var buttonForm = new ButtonForm();
            buttonForm.AddButtonRow("Обрати", venue!.Id.ToString());
            await Device.Send(FormatVenueDetails(venue), buttonForm, parseMode: ParseMode.Html);
        }
        else
        {
            await NotifyVenueNotFoundError();
        }
    }

    private string FormatVenueDetails(Venue venue)
    {
        return FormatVenue.Format(venue.Name, venue.City, venue.LocationUrl, venue.InstagramUrl);
    }

    private bool TryGetVenue(string input, out Venue? venue)
    {
        venue = null;
        if (!Guid.TryParse(input, out Guid selectedVenue) || selectedVenue == Guid.Empty)
        {
            return false;
        }

        venue = _venues.FirstOrDefault(v => v.Id == selectedVenue);
        return venue != null;
    }

    private async Task NotifyVenueNotFoundError()
    {
        await Device.Send("Помилка! Такого місця проведення не знайдено!");
    }
}
