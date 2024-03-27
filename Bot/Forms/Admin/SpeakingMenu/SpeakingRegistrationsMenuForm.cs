using System.Globalization;

using Application.Extensions;
using Application.Registrations.Commands.ForceModifyStatus;
using Application.Registrations.Queries.GetUserRegistrations;

using Bot.Extensions;
using Bot.Forms.Common.Base;

using Domain.Entities;
using Domain.Enums;

using Humanizer;

using MediatR;

using Telegram.Bot.Types;

using TelegramBotBase.Args;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingRegistrationsMenuForm : ControlPanelForm<Registration>
{
    private Speaking? _selectedSpeaking;

    private readonly ActionButton _approveCardButton;
    private readonly ActionButton _approveCashButton;

    public SpeakingRegistrationsMenuForm(IMediator mediator)
    {
        _mediator = mediator;
        _approveCardButton = new ActionButton(
            "Підтвердити оплату карткою💳",
            "confirmCard",
            CashPaymentConfirmation
        );

        _approveCashButton = new ActionButton(
            "Підтвердити оплату готівкою💴",
            "confirmCash",
            CardPaymentConfirmation
        );

        _mButtons.NoItemsLabel = "Реєстрацій на цей івент немає";
    }

    protected override Task ListForm_Init(object sender, InitEventArgs e)
    {
        if (e.Args.Length > 0)
        {
            try
            {
                _selectedSpeaking = (Speaking)e.Args[0];
            }
            catch
            {
                throw;
            }
        }
        _listTitle = $"Список реєстрацій на {_selectedSpeaking?.Title}";
        return base.ListForm_Init(sender, e);
    }

    protected override async Task SetEntities()
    {
        var result = await _mediator.Send(
            new GetSpeakingRegistrations() { SpeakingId = _selectedSpeaking?.Id ?? Guid.Empty }
        );
        result.Match((registrations) => _entities = registrations, (error) => _entities = new());
    }

    protected override string GetButtonName(Registration registration)
    {
        return $"{registration.User.FirstName} {registration.User.LastName} "
            + $"Статус платежу: {registration.PaymentStatus.GetDescription()}";
    }

    protected override string GetControlTitle(Registration registration)
    {
        var registrationDate = registration.RegistrationDate.ToLocalTime();
        var transferTicketStatus = registration.User.TransferTicket ? "Присутній✅" : "Відсутній🛑";
        return $"Інформація про реєстрацію\n"
            + $"Дата реєстрації: {registrationDate}, {registrationDate.Humanize(culture: new CultureInfo("uk-UA"))}\n"
            + $"Статус: {registration.PaymentStatus.GetDescription()}\n\n"
            + $"Інформація про користувача.\n"
            + $"Ім'я: {registration.User.FirstName}\nПрізвище: {registration.User.LastName}\n"
            + $"Номер телефону: {registration.User.PhoneNumber}\n"
            + $"Рівень англійської: {registration.User.EnglishLevel}\n"
            + $"Квиток переносу: {transferTicketStatus}\n\n"
            + $"<a href=\"tg://user?id={registration.User.TelegramId}\">Зв'язатись з користувачем</a>";
    }

    protected override ActionButton[] SetControlButtons(Registration entity)
    {
        if (entity.PaymentStatus == PaymentStatus.ToBeApproved)
        {
            return new ActionButton[] { _approveCardButton, _approveCashButton };
        }
        return Array.Empty<ActionButton>();
    }

    private async Task PaymentConfirmation(PaymentStatus paymentStatus, string successMessage)
    {
        var result = await _mediator.Send(
            new ForceModifyStatusCommand
            {
                PaymentStatus = paymentStatus,
                RegistrationId = _selectedEntity?.Id ?? Guid.Empty
            }
        );
        await result.Match(
            (_) =>
                Device.Send(
                    string.Format(
                        successMessage,
                        _selectedEntity?.User.FirstName,
                        _selectedEntity?.Speaking.GetName()
                    )
                ),
            (error) => Device.Send(error.Message)
        );
        LeaveLastMessage();
    }

    private async Task CardPaymentConfirmation()
    {
        await PaymentConfirmation(
            PaymentStatus.ToBePaidByCash,
            "Ви успішно підтвердили, що користувач {0} оплатить/оплатив готівкою реєстрацію на {1}"
        );
    }

    private async Task CashPaymentConfirmation()
    {
        await PaymentConfirmation(
            PaymentStatus.PaidByCard,
            "Ви успішно підтвердили оплату карткою користувача {0}, який зареєструвався на {1}"
        );
    }
}
