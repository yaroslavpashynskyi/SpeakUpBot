using Application.Registrations.Commands.CancelRegistration;
using Application.Registrations.Commands.ConfirmPayment;
using Application.Registrations.Commands.RestoreRegistration;
using Application.Registrations.Queries.GetUserRegistrations;

using Bot.Extensions;
using Bot.Forms.Common.Base;

using Domain.Entities;
using Domain.Enums;

using MediatR;

using Telegram.Bot.Types;

namespace Bot.Forms.Member.RegistrationMenu;

public class MemberRegistrationListForm : ControlPanelForm<Registration>
{
    private readonly ActionButton _confirmPaymentButton;
    private readonly ActionButton _cancelButton;
    private readonly ActionButton _restoreRegistrationButton;

    public MemberRegistrationListForm(IMediator mediator)
    {
        _mediator = mediator;

        _confirmPaymentButton = new ActionButton(
            "Підтвердити оплату✅",
            "cancelRegistration",
            ConfirmPayment
        );
        _cancelButton = new ActionButton(
            "Скасувати реєстрацію⚠️",
            "confirmRegistration",
            CancelRegistration
        );
        _restoreRegistrationButton = new ActionButton(
            "Відновити реєстрацію🔄",
            "restoreRegistration",
            RestoreRegistration
        );
        _mButtons.NoItemsLabel = "У вас поки немає реєстрацій";
    }

    protected override string GetButtonName(Registration registration)
    {
        return $"{registration.Speaking.Title}. {registration.PaymentStatus.GetDescription()}";
    }

    protected override Task SetEntities()
    {
        _request = new GetUserRegistrations() { UserTelegramId = Device.DeviceId };
        return base.SetEntities();
    }

    protected override string GetControlTitle(Registration registration)
    {
        DateTime registrationDateTime = registration.RegistrationDate.ToLocalTime();
        DateTime speakingStartDateTime = registration.Speaking.TimeOfEvent.ToLocalTime();
        DateTime endTime = speakingStartDateTime.AddMinutes(registration.Speaking.DurationMinutes);

        string formattedStartTime = speakingStartDateTime.ToString("dd.MM.yyyy HH:mm");
        string formattedEndTime = endTime.ToString("HH:mm");

        string formattedTimeRange = $"{formattedStartTime}-{formattedEndTime}";
        return $"Реєстрація на {registration.Speaking.Title}\n\n"
            + $"Статус платежу: {registration.PaymentStatus.GetDescription()}\n"
            + $"Дата та час реєстрації: {registrationDateTime.ToString("dd.MM.yyyy HH:mm")}\n"
            + $"Дата та час івенту: {formattedTimeRange}\n"
            + $"Статус івенту: {registration.Speaking.Status.GetDescription()}";
    }

    private async Task ConfirmPayment()
    {
        var result = await _mediator.Send(
            new ConfirmPaymentCommand { Registration = _selectedEntity! }
        );

        await result.Match(
            (_) =>
                Device.Send(
                    $"Ви підтвердили оплату на {_selectedEntity!.Speaking.Title}.\n"
                        + "Очікуйте на підтвердження платежу з боку організатора.\n"
                        + "Організатор може з вами зв'язатись для уточнення інформації."
                ),
            (error) => Device.Send(error.Message)
        );
    }

    private async Task RestoreRegistration()
    {
        var result = await _mediator.Send(
            new RestoreRegistrationCommand { Registration = _selectedEntity! }
        );

        await result.Match(
            (paymentStatus) =>
                Device.Send(
                    $"Ви відновили реєстрацію на {_selectedEntity!.Speaking.Title}"
                        + (
                            paymentStatus == PaymentStatus.PaidByTransferTicket
                                ? "\nВаш квиток переносу використався, тому ваша реєстрація вже оплачена!"
                                : ""
                        )
                ),
            (error) => Device.Send(error.Message)
        );
    }

    private async Task CancelRegistration()
    {
        var result = await _mediator.Send(
            new CancelRegistrationCommand { Registration = _selectedEntity! }
        );

        await result.Match(HandleCancellation, (error) => Device.Send(error.Message));
    }

    private Task<Message> HandleCancellation(CancelledRegistrationResult cancelResult)
    {
        string message = $"Ви скасували реєстрацію на {_selectedEntity!.Speaking.Title}. ";
        if (
            _selectedEntity.PaymentStatus != PaymentStatus.Pending
            && _selectedEntity.PaymentStatus != PaymentStatus.ToBePaidByCash
        )
            message += cancelResult.TransferTicketGained
                ? "Ви встигли скасувати за 48 до початку івенту, тому на наступний івент, запис безкоштовний."
                : "Ви не встигли скасувати за 48 до початку івенту, тому кошти згорають.";

        return Device.Send(message);
    }

    protected override ActionButton[] SetControlButtons(Registration registration)
    {
        return registration.Speaking.Status == SpeakingStatus.Completed
            ? (Array.Empty<ActionButton>())
            : registration.PaymentStatus switch
            {
                PaymentStatus.Pending => new[] { _confirmPaymentButton, _cancelButton },
                PaymentStatus.ToBeApproved => Array.Empty<ActionButton>(),
                PaymentStatus.Cancelled => new ActionButton[] { _restoreRegistrationButton },
                _ => new ActionButton[] { _cancelButton },
            };
    }
}
