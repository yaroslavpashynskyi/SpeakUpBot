using Application.Venues.Commands.CreateVenue;

using Bot.Utils;

using MediatR;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.VenueMenu;

public class CreateVenueForm : AutoCleanForm
{
    public CreateVenueCommand VenueData { get; set; } = new();
    private readonly IMediator _mediator;

    public CreateVenueForm(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task Load(MessageResult message)
    {
        if (string.IsNullOrWhiteSpace(message.MessageText) || message.Handled)
        {
            return;
        }

        foreach (var property in typeof(CreateVenueCommand).GetProperties())
        {
            if (property.GetValue(VenueData) != null)
                continue;

            if (property.PropertyType == typeof(string))
            {
                if (message.MessageText.Length > 50)
                {
                    await Device.Send("Назва не повинна перебільшувати 50 символів");
                    break;
                }
                property.SetValue(VenueData, message.MessageText);
                break;
            }
            else if (property.PropertyType == typeof(Uri))
            {
                Uri.TryCreate(message.MessageText, UriKind.Absolute, out var uri);

                if (uri == null)
                {
                    await Device.Send("Будь ласка, надайте дійсне посилання");
                    break;
                }

                property.SetValue(VenueData, uri);
                break;
            }
        }
        return;
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        switch (message.RawData)
        {
            case "confirm":
                foreach (var property in typeof(CreateVenueCommand).GetProperties())
                {
                    if (property.GetValue(VenueData) == null)
                        return;
                }

                await _mediator.Send(VenueData);
                await this.NavigateTo<VenueMenuForm>();
                break;
            case "startOver":
                VenueData = new();
                break;
        }

        await message.ConfirmAction();
    }

    public override async Task Render(MessageResult message)
    {
        if (VenueData.Name == null)
        {
            await Device.Send("Напишіть назву закладу");
            return;
        }
        if (VenueData.City == null)
        {
            await Device.Send("Напишіть місто, в якому знаходиться заклад");
            return;
        }
        if (VenueData.Location == null)
        {
            await Device.Send("Надішліть посилання на точне місцезнаходження закладу");
            return;
        }
        if (VenueData.Instagram == null)
        {
            await Device.Send("Надішліть посилання на інстраграм сторінку закладу");
            return;
        }

        var bf = new ButtonForm();
        bf.AddButtonRow(
            new ButtonBase("Так, все вірно", "confirm"),
            new ButtonBase("Почати спочатку", "startOver")
        );
        bf.AddButtonRow();

        await Device.Send(
            "Чи всі дані вірні?\n"
                + FormatVenue.Format(
                    VenueData.Name,
                    VenueData.City,
                    VenueData.Location,
                    VenueData.Instagram
                ),
            bf,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
        );
    }
}
