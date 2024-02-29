using System.Security.Cryptography;

using Application.Speakings.Queries.GetAllSpeakings;

using Bot.Extensions;
using Bot.Forms.Common.Base;

using Domain.Entities;
using Domain.Enums;

using MediatR;

using Telegram.Bot.Types;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class SpeakingListForm : ListItemsForm<Speaking>
{
    private readonly ButtonBase _registrationListButton = new ButtonBase(
        "Список реєстрацій",
        "registrationList"
    );
    private readonly ButtonBase _showPostButton = new ButtonBase("Переглянути пост", "showPost");
    private Speaking? _selectedSpeaking;

    private Message? _messageToDelete;

    public SpeakingListForm(IMediator mediator)
    {
        _mediator = mediator;

        _request = new GetAllSpeakingsWithRegistrations();
        _listTitle = "Меню спікінгу";
        _backForm = typeof(SpeakingMenuForm);
    }

    protected override string GetButtonName(Speaking speaking)
    {
        return $"{speaking.Title}, {speaking.Venue.City} "
            + $"({PaidRegistrationsCount(speaking)}/"
            + $"{speaking.Registrations.Count(r => r.PaymentStatus != PaymentStatus.Cancelled)}/{speaking.Seats})";
    }

    private static int PaidRegistrationsCount(Speaking speaking)
    {
        var paidStatuses = new List<PaymentStatus>()
        {
            PaymentStatus.PaidByCard,
            PaymentStatus.PaidByTransferTicket,
            PaymentStatus.ToBePaidByCash
        };

        return speaking.Registrations.Count(r => paidStatuses.Contains(r.PaymentStatus));
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        if (message.RawData == _registrationListButton.Value)
        {
            await this.NavigateTo<SpeakingRegistrationsMenuForm>(_selectedSpeaking);
            return;
        }
        else if (message.RawData == _showPostButton.Value && _selectedSpeaking != null)
        {
            await this.NavigateTo<ShowSpeakingPostForm>(_selectedSpeaking);
            return;
        }
    }

    protected override async Task HandleEntity(Speaking speaking)
    {
        _selectedSpeaking = speaking;

        if (_messageToDelete != null)
            await Device.DeleteMessage(_messageToDelete);

        var bf = new ButtonForm();
        bf.AddButtonRow(_registrationListButton);
        bf.AddButtonRow(_showPostButton);
        _messageToDelete = await Device.Send(
            $"{speaking.Title}, {speaking.Venue.City}\n\n"
                + $"Оплатили: {PaidRegistrationsCount(speaking)}\n"
                + $"Зареєстровані: {speaking.Registrations.Count(r => r.PaymentStatus != PaymentStatus.Cancelled)}\n"
                + $"Максимальна к-сть місць: {speaking.Seats}",
            bf
        );
    }
}
