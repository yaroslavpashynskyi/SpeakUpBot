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
using Telegram.Bot.Types.Enums;

using TelegramBotBase.Args;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Member.RegistrationMenu;

public class MemberRegistrationList : ListItemsForm<Registration>
{
    private bool _registrationControlMode = false;
    private Registration? _selectedRegistration;
    private readonly ButtonForm _controlModeForm = new ButtonForm();
    private Message? _messageToClear;

    private ButtonBase? _backToMenuButton;
    private readonly ButtonBase _confirmPaymentButton =
        new("Підтвердити оплату✅", "cancelRegistration");
    private readonly ButtonBase _cancelButton =
        new("Скасувати реєстрацію⚠️", "confirmRegistration");
    private readonly ButtonBase _restoreRegistrationButton =
        new("Відновити реєстрацію🔄", "restoreRegistration");

    public MemberRegistrationList(IMediator mediator)
    {
        _mediator = mediator;
        _mButtons.KeyboardType = EKeyboardType.InlineKeyBoard;
        _mButtons.MessageParseMode = ParseMode.Html;
        _mButtons.ButtonClicked += OnControlMode_ButtonClicked;
        _listTitle = "Мої записи";
        _mButtons.NoItemsLabel = "У вас поки немає реєстрацій";

        _controlModeForm.AddButtonRow(new ButtonBase("◀️Повернутись до списку", "backToList"));
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

    protected override Task List_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        return _registrationControlMode ? Task.CompletedTask : base.List_ButtonClicked(sender, e);
    }

    private async Task OnControlMode_ButtonClicked(object sender, ButtonClickedEventArgs e)
    {
        if (_messageToClear != null)
        {
            await Device.DeleteMessage(_messageToClear);
            _messageToClear = null;
        }

        if (_registrationControlMode)
        {
            if (e.Button == null)
                return;

            if (e.Button.Value == "backToList")
            {
                await RenderRegistrationList();
                return;
            }

            if (_selectedRegistration == null)
                return;

            if (e.Button.Value == _cancelButton.Value)
            {
                var result = await _mediator.Send(
                    new CancelRegistrationCommand { Registration = _selectedRegistration }
                );

                _messageToClear = await result.Match(
                    HandleCancellation,
                    (error) => Device.Send(error.Message)
                );
                await RenderRegistrationList();
            }
            else if (e.Button.Value == _restoreRegistrationButton.Value)
            {
                var result = await _mediator.Send(
                    new RestoreRegistrationCommand { Registration = _selectedRegistration }
                );

                _messageToClear = await result.Match(
                    (paymentStatus) =>
                        Device.Send(
                            $"Ви відновили реєстрацію на {_selectedRegistration.Speaking.Title}"
                                + (
                                    paymentStatus == PaymentStatus.PaidByTransferTicket
                                        ? "\nВаш квиток переносу використався, тому ваша реєстрація вже оплачена!"
                                        : ""
                                )
                        ),
                    (error) => Device.Send(error.Message)
                );
                await RenderRegistrationList();
            }
            else if (e.Button.Value == _confirmPaymentButton.Value)
            {
                var result = await _mediator.Send(
                    new ConfirmPaymentCommand { Registration = _selectedRegistration }
                );

                _messageToClear = await result.Match(
                    (unit) =>
                        Device.Send(
                            $"Ви підтвердили оплату на {_selectedRegistration.Speaking.Title}.\n"
                                + "Очікуйте на підтвердження платежу з боку організатора.\n"
                                + "Організатор може з вами зв'язатись для уточнення інформації."
                        ),
                    (error) => Device.Send(error.Message)
                );
                await RenderRegistrationList();
            }
        }
    }

    private Task<Message> HandleCancellation(CancelledRegistrationResult cancelResult)
    {
        string message = $"Ви скасували реєстрацію на {_selectedRegistration!.Speaking.Title}. ";
        if (
            _selectedRegistration.PaymentStatus != PaymentStatus.Pending
            && _selectedRegistration.PaymentStatus != PaymentStatus.ToBePaidByCash
        )
            message += cancelResult.TransferTicketGained
                ? "Ви встигли скасувати за 48 до початку спікінгу, тому на наступний спікінг, запис безкоштовний."
                : "Ви не встигли скасувати за 48 до початку спікінгу, тому кошти згорають.";

        return Device.Send(message);
    }

    private async Task RenderRegistrationList()
    {
        _registrationControlMode = false;
        _selectedRegistration = null;
        _mButtons.EnablePaging = true;

        var bf = new ButtonForm();
        await SetEntities();
        foreach (var registration in _entities)
            bf.AddButtonRow(GetButtonName(registration), registration.Id.ToString());

        _mButtons.DataSource.ButtonForm = bf;
        _mButtons.HeadLayoutButtonRow = new(_backToMenuButton);
        _mButtons.Title = _listTitle;
        _mButtons.Updated();
    }

    protected override Task HandleEntity(Registration registration)
    {
        RenderRegistrationControlPanel(registration);
        return Task.CompletedTask;
    }

    private void RenderRegistrationControlPanel(Registration registration)
    {
        _registrationControlMode = true;
        _selectedRegistration = registration;
        _mButtons.EnablePaging = false;

        ButtonBase[] additionalButtons =
            registration.Speaking.Status == SpeakingStatus.Completed
                ? (Array.Empty<ButtonBase>())
                : registration.PaymentStatus switch
                {
                    PaymentStatus.Pending => new[] { _confirmPaymentButton, _cancelButton },
                    PaymentStatus.ToBeApproved => Array.Empty<ButtonBase>(),
                    PaymentStatus.Cancelled => new ButtonBase[] { _restoreRegistrationButton },
                    _ => new ButtonBase[] { _cancelButton },
                };

        var bf = _controlModeForm.Duplicate();
        bf.AddSplitted(additionalButtons, 1);
        _mButtons.DataSource.ButtonForm = bf;

        _backToMenuButton = _mButtons.HeadLayoutButtonRow.ToList().First();
        _mButtons.HeadLayoutButtonRow = null;
        _mButtons.Title = FormatRegistration(registration);
        _mButtons.Updated();
    }

    private string FormatRegistration(Registration registration)
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
            + $"Дата та час спікінгу: {formattedTimeRange}\n"
            + $"Статус спікінгу: {registration.Speaking.Status.GetDescription()}";
    }
}
