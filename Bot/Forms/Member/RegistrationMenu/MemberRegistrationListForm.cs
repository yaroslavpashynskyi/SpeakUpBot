﻿using System.Collections.Immutable;

using Application.Common.Models;
using Application.Extensions;
using Application.Registrations.Commands.CancelRegistration;
using Application.Registrations.Commands.ConfirmPayment;
using Application.Registrations.Commands.RestoreRegistration;
using Application.Registrations.Queries.GetUserRegistrations;

using Bot.Extensions;
using Bot.Forms.Common.Base;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;

using MediatR;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using TelegramBotBase.Base;

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
            "confirmRegistration",
            ConfirmPayment
        );
        _cancelButton = new ActionButton(
            "Скасувати реєстрацію⚠️",
            "cancelRegistration",
            CancelRegistration
        );
        _restoreRegistrationButton = new ActionButton(
            "Активувавати реєстрацію🔄",
            "activateRegistration",
            ActivateRegistration
        );
        _listTitle = "Мої записи";
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

    public override Task Action(MessageResult message)
    {
        if (!Guid.TryParse(message.RawData, out var speakingId))
        {
            return Task.CompletedTask;
        }

        var registration = _entities.FirstOrDefault(r => r.Speaking.Id == speakingId);
        if (registration == null)
        {
            return Task.CompletedTask;
        }

        _backForm = typeof(MemberMenuForm);
        RenderControlPanel(registration);

        return Task.CompletedTask;
    }

    protected override string GetControlTitle(Registration registration)
    {
        DateTime registrationDateTime = registration.RegistrationDate.ToLocalTime();
        DateTime speakingStartDateTime = registration.Speaking.TimeOfEvent.ToLocalTime();
        DateTime endTime = speakingStartDateTime.AddMinutes(registration.Speaking.DurationMinutes);

        string formattedStartTime = speakingStartDateTime.ToString("dd.MM.yyyy HH:mm");
        string formattedEndTime = endTime.ToString("HH:mm");

        string formattedTimeRange = $"{formattedStartTime}-{formattedEndTime}";
        var venue = registration.Speaking.Venue;
        return $"Реєстрація на {registration.Speaking.Title}\n\n"
            + $"Місце проведення: <a href=\"{venue.InstagramUrl}\">{venue.Name}</a>"
            + $" в м. <a href=\"{venue.LocationUrl}\">{venue.City}</a>\n"
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
                    $"Ви підтвердили оплату на {_selectedEntity!.Speaking.GetName()}.\n"
                        + "Очікуйте на підтвердження платежу з боку організатора.\n"
                        + "Організатор може з вами зв'язатись для уточнення інформації."
                ),
            (error) => Device.Send(error.Message)
        );
        LeaveLastMessage();
    }

    private async Task ActivateRegistration()
    {
        var result = await _mediator.Send(
            new RestoreRegistrationCommand { Registration = _selectedEntity! }
        );

        await result.Match(
            (paymentStatus) =>
            {
                var message = paymentStatus switch
                {
                    PaymentStatus.PaidByTransferTicket
                        => "Ваш квиток переносу використався, тому ваша реєстрація вже оплачена!",
                    PaymentStatus.Pending
                        => "Вам потрібно перерахувати"
                            + $" {_selectedEntity?.Speaking.Price} грн на картку:\n4441111137379347\n"
                            + $"Після цього підтвердити свою оплату в меню записів.",
                    PaymentStatus.InReserve
                        => "На жаль, всі місця на цей івент зайняті. Ви зараз знаходитесь в резерві."
                            + " Ми вам повідомимо, якщо звільниться місце.",
                    _ => "Непередбачений статус платежу. Зверніться до адміністратора."
                };
                return Device.Send(
                    $"Ви активували реєстрацію на {_selectedEntity!.Speaking.GetName()}\n{message}"
                );
            },
            (error) => Device.Send(error.Message)
        );
        LeaveLastMessage();
    }

    private async Task CancelRegistration()
    {
        var result = await _mediator.Send(
            new CancelRegistrationCommand { Registration = _selectedEntity! }
        );

        await result.Match(HandleCancellation, (error) => Device.Send(error.Message));
        LeaveLastMessage();
    }

    private Task<Message> HandleCancellation(CancelledRegistrationResult cancelResult)
    {
        string message = $"Ви скасували реєстрацію на {_selectedEntity!.Speaking.GetName()}. ";
        if (
            _selectedEntity.PaymentStatus != PaymentStatus.Pending
            && _selectedEntity.PaymentStatus != PaymentStatus.ToBePaidByCash
        )
            message += cancelResult.TransferTicketGained
                ? "Ви встигли скасувати за 48 годин до початку івенту, тому запис на наступний івент <b>безкоштовний.</b>"
                : "Ви не встигли скасувати за 48 годин до початку івенту, тому кошти <b>згорають.</b>";

        return Device.Send(message, parseMode: ParseMode.Html);
    }

    protected override ActionButton[] SetControlButtons(Registration registration)
    {
        return registration.Speaking.Status == SpeakingStatus.Completed
            ? (Array.Empty<ActionButton>())
            : registration.PaymentStatus switch
            {
                PaymentStatus.Pending => new[] { _confirmPaymentButton, _cancelButton },
                PaymentStatus.ToBeApproved => Array.Empty<ActionButton>(),
                PaymentStatus.Cancelled => new[] { _restoreRegistrationButton },
                PaymentStatus.InReserve
                    => ShowActivateButton(registration)
                        ? new[] { _restoreRegistrationButton }
                        : Array.Empty<ActionButton>(),
                _ => new[] { _cancelButton },
            };
    }

    private bool ShowActivateButton(Registration registration)
    {
        Result<List<Registration>, Error> result = _mediator
            .Send(new GetSpeakingRegistrations() { SpeakingId = registration.SpeakingId })
            .Result;
        if (result.IsSuccess)
        {
            ImmutableList<Guid> inReserve = result.Value!
                .Where(
                    r =>
                        r.SpeakingId == registration.SpeakingId
                        && r.PaymentStatus == PaymentStatus.InReserve
                )
                .OrderBy(r => r.RegistrationDate)
                .Select(r => r.Id)
                .ToImmutableList();
            var availableSeats =
                registration.Speaking.Seats
                - result.Value!.Count(
                    r => r.PaymentStatus is not (PaymentStatus.Cancelled or PaymentStatus.InReserve)
                );
            return inReserve.IndexOf(registration.Id) < availableSeats;
        }

        return false;
    }
}
