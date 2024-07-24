using Application.Common.Models;
using Application.Registrations.Queries.DoesUserRegistered;
using Application.Speakings.Queries.GetAllSpeakings;

using Bot.Extensions;
using Bot.Forms.Common.Base;
using Bot.Forms.Member.RegistrationMenu;

using Domain.Common;
using Domain.Entities;

using MediatR;

using Telegram.Bot.Types;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Member;

public class FutureSpeakingsForm : ListItemsForm<Speaking>
{
    Message[]? lastPostMessages = Array.Empty<Message>();
    private bool _isRegistered;

    public FutureSpeakingsForm(IMediator mediator)
    {
        _mediator = mediator;

        _request = new GetAllSpeakingsWithVenue();
        _listTitle = "Майбутні івенти";
        _mButtons.NoItemsLabel = "Наразі ніяких івентів поки не планується😞";
        _filter = s => s.TimeOfEvent.ToLocalTime() > DateTime.Now;
    }

    protected override string GetButtonName(Speaking speaking)
    {
        return $"{speaking.Title} ({speaking.TimeOfEvent.ToLocalTime().ToString("dd.MM")}),"
            + $" {speaking.Venue.City}";
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        if (
            Guid.TryParse(message.RawData, out var speakingId)
            && _entities.Any(s => s.Id == speakingId)
        )
        {
            if (_isRegistered)
                await this.NavigateTo<MemberRegistrationListForm>();
            else
            {
                this.OldMessages = this.OldMessages.TakeLast(1).ToList();
                await this.NavigateTo<CreateRegistrationForm>();
            }
        }
    }

    protected override async Task HandleEntity(Speaking speaking)
    {
        if (lastPostMessages?.Length > 0)
        {
            foreach (var postMessage in lastPostMessages)
            {
                await Device.DeleteMessage(postMessage);
            }
        }
        Message[]? sentPost = await Device.SendSpeakingPost(speaking);
        if (sentPost == null)
            return;

        Result<bool, Error> result = await _mediator.Send(
            new DoesUserRegisteredQuery()
            {
                SpeakingId = speaking.Id,
                TelegramUserId = Device.DeviceId
            }
        );

        var button = new ButtonBase("Зареєструватись", speaking.Id.ToString());
        _isRegistered = result.Match(
            (registered) =>
            {
                if (registered)
                    button.Text = "Перейти до запису";
                return registered;
            },
            error =>
            {
                _ = Device.Send(error.Message);
                return false;
            }
        );

        if (result.IsError)
            return;

        var bf = new ButtonForm();
        bf.AddButtonRow(button);
        var message = _isRegistered
            ? "Ви вже зареєстровані на цей івент. Щоб перейти до запису натисність кнопку нижче"
            : "Для реєстрації натисніть кнопку нижче";

        sentPost = sentPost.Append(await Device.Send(message, bf)).ToArray();

        foreach (var postMessage in sentPost)
        {
            AddMessage(postMessage);
        }
        lastPostMessages = sentPost;
    }
}
