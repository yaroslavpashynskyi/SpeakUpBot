using Application.Extensions;
using Application.Registrations.Commands.ConfirmPayment;
using Application.Registrations.Commands.CreateRegistration;
using Application.Registrations.Queries.DoesUserRegistered;
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
    private Registration? _createdRegistration;
    private readonly ButtonBase _confirmButton = new("Підтвердити оплату🟢", "confirmNow");
    private bool _isRegistered;

    public CreateRegistrationForm(IMediator mediator)
    {
        _mediator = mediator;

        _listTitle = "Оберіть івент на який бажаєте записатись";
        _filter = s => s.TimeOfEvent > DateTime.Now;
        _mButtons.NoItemsLabel = "Наразі, немає івентів, на які ви б могли записатись😔";
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

        var result = await _mediator.Send(
            new DoesUserRegisteredQuery()
            {
                SpeakingId = speaking.Id,
                TelegramUserId = Device.DeviceId
            }
        );
        if (result.IsError)
            return;

        var bf = new ButtonForm();
        _isRegistered = result.Value;
        var button = new ButtonBase(
            _isRegistered ? "Перейти до запису" : "Підтвердити",
            speaking.Id.ToString()
        );
        bf.AddButtonRow(button);
        var confirm = await Device.Send(
            _isRegistered
                ? $"Ви зареєстровані {speaking.Title}, але запис скасовано. Ви можете відновити його, перейшовши до запису."
                : $"Бажаєте записатись на {speaking.Title}?",
            bf
        );

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
        var speakingName = _createdRegistration?.Speaking.GetName();
        await message.ConfirmAction();

        if (message.RawData == _confirmButton.Value && _createdRegistration != null)
        {
            await MessageCleanup();
            var confirmResult = await _mediator.Send(
                new ConfirmPaymentCommand() { Registration = _createdRegistration }
            );

            await confirmResult.Match(
                (_) =>
                    Device.Send(
                        $"Ви успішно зареєструвались та підтвердити оплату на {speakingName},"
                            + $" очікуйте поки організатор не підтвердить вашу оплату."
                    ),
                (error) => Device.Send(error.Message)
            );
            LeaveLastMessage();

            await this.NavigateTo<MemberMenuForm>();
            return;
        }

        Guid.TryParse(message.RawData, out var speakingId);
        var speaking = _entities.Where(_filter).FirstOrDefault(s => s.Id == speakingId);
        if (speaking == null)
        {
            await Device.Send("Такого івенту не існує!");
            return;
        }

        if (_isRegistered)
        {
            await this.NavigateTo<MemberRegistrationListForm>();
            return;
        }
        var result = await _mediator.Send(
            new CreateRegistrationCommand { Speaking = speaking, UserTelegramId = Device.DeviceId }
        );

        await result.Match(SendSuccess, async (error) => await Device.Send(error.Message));
    }

    private async Task SendSuccess(Registration registration)
    {
        _createdRegistration = registration;
        await CleanPostMessages();
        await MessageCleanup();

        string message =
            $"Вітаємо! Ви успішно зареєструвались на {registration.Speaking.GetName()}.\n";
        string footer =
            "Увага! При скасуванні оплаченої реєстрації за 48 годин до початку івенту,"
            + " наступний івент безкоштовний. Інакше, кошти згорають";

        if (registration.PaymentStatus == PaymentStatus.PaidByTransferTicket)
        {
            await Device.Send(
                message
                    + "Ваш квиток переносу використався, тому ваша реєстрація вже оплачена!\n"
                    + footer
            );
            LeaveLastMessage();

            await this.NavigateTo<MemberMenuForm>();
        }
        else
        {
            var bf = new ButtonForm();

            bf.AddButtonRow(_confirmButton);

            await Device.Send(
                message
                    + $"За вами закріплене місце найближчу годину, впродовж якої необхідно здійснити оплату в"
                    + $" {registration.Speaking.Price} грн на картку:\n4441111137379347\n\nМаємо повідомити,"
                    + " що кількість місць на івент обмежена, тому хто first pay, first served. \n\nУчасть у "
                    + "заході підтверджується після оплати та підтвердження наявності місця організаторами.\n\n"
                    + footer,
                bf,
                parseMode: ParseMode.Html
            );
        }

        RemoveControl(_mButtons);
    }
}
