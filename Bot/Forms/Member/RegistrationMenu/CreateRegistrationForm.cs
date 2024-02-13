using Application.Registrations.Commands.CreateRegistration;
using Application.Speakings.Queries.GetUserUnregisteredSpeakings;

using Bot.Extensions;
using Bot.Forms.Common.Base;

using Domain.Entities;
using Domain.Enums;

using MediatR;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Member.RegistrationMenu;

public class CreateRegistrationForm : ListItemsForm<Speaking>
{
    private Message[]? _lastPostMessages = Array.Empty<Message>();
    private readonly ButtonBase _confirmButton = new("Зрозуміло", "confirm");

    public CreateRegistrationForm(IMediator mediator)
    {
        _mediator = mediator;

        _listTitle = "Оберіть спікінг на який бажаєте записатись";
        _filter = s => s.TimeOfEvent > DateTime.Now;
        _mButtons.NoItemsLabel = "Наразі, немає спікінгів, на які ви б могли записатись😔";
    }

    protected override Task SetEntities()
    {
        _request = new GetUserUnregisteredSpeakings() { UserTelegramId = Device.DeviceId };
        return base.SetEntities();
    }

    protected override string GetButtonName(Speaking speaking)
    {
        return $"{speaking.Title} ({speaking.TimeOfEvent.ToString("dd.MM")}),"
            + $" {speaking.Venue.City}";
    }

    protected override async Task HandleEntity(Speaking speaking)
    {
        await CleanPostMessages();
        Message[]? sentPost = await Device.SendSpeakingPost(speaking);
        if (sentPost == null)
            return;

        var bf = new ButtonForm();
        bf.AddButtonRow("Підтвердити", speaking.Id.ToString());
        var confirm = await Device.Send($"Бажаєте записатись на {speaking.Title}?", bf);

        _lastPostMessages = sentPost;
        _lastPostMessages = _lastPostMessages.Append(confirm).ToArray();
        foreach (var postMessage in sentPost)
        {
            AddMessage(postMessage);
        }
    }

    private async Task CleanPostMessages()
    {
        if (_lastPostMessages?.Length > 0)
        {
            foreach (var postMessage in _lastPostMessages)
            {
                await Device.DeleteMessage(postMessage);
            }
        }
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        if (message.RawData == _confirmButton.Value)
        {
            await this.NavigateTo<MemberMenuForm>();
            return;
        }

        Guid.TryParse(message.RawData, out var speakingId);
        var speaking = _entities.Where(_filter).FirstOrDefault(s => s.Id == speakingId);
        if (speaking == null)
        {
            await Device.Send("Такого спікінгу не існує!");
            return;
        }
        var result = await _mediator.Send(
            new CreateRegistrationCommand { Speaking = speaking, UserTelegramId = Device.DeviceId }
        );

        await result.Match(SendSuccess, async (error) => await Device.Send(error.Message));
    }

    private async Task SendSuccess(Registration registration)
    {
        await CleanPostMessages();
        await MessageCleanup();

        var bf = new ButtonForm();
        bf.AddButtonRow(_confirmButton);

        string message = $"Вітаємо! Ви успішно зареєструвались на {registration.Speaking.Title}.\n";
        string footer =
            "Увага! При скасуванні оплаченої реєстрації за 48 годин до початку спікінгу,"
            + " наступний спікінг безкоштовний. Інакше, кошти згорають";

        if (registration.PaymentStatus == PaymentStatus.PaidByTransferTicket)
            await Device.Send(
                footer =
                    message
                    + $"Ваш квиток переносу використався, тому ваша реєстраці вже оплачена!\n"
                    + footer,
                bf
            );
        else
        {
            await Device.Send(
                message
                    + $"Тепер вам потрібно перерахувати {registration.Speaking.Price}грн на карту в повідомлені нижче.\n"
                    + $"Після того як оплатили, потрібно підтвердити оплату через меню записів.\n"
                    + footer,
                bf
            );
            await Device.Send("<code>4441111137379347</code>", parseMode: ParseMode.Html);
        }

        RemoveControl(_mButtons);
    }
}
