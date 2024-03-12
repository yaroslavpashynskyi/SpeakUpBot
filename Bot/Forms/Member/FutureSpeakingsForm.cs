using Application.Speakings.Queries.GetAllSpeakings;

using Bot.Extensions;
using Bot.Forms.Common.Base;

using Domain.Entities;

using MediatR;

using Telegram.Bot.Types;

namespace Bot.Forms.Member;

public class FutureSpeakingsForm : ListItemsForm<Speaking>
{
    Message[]? lastPostMessages = Array.Empty<Message>();

    public FutureSpeakingsForm(IMediator mediator)
    {
        _mediator = mediator;

        _request = new GetAllSpeakingsWithVenue();
        _listTitle = "Майбутні івенти";
        _filter = s => s.TimeOfEvent.ToLocalTime() > DateTime.Now;
    }

    protected override string GetButtonName(Speaking speaking)
    {
        return $"{speaking.Title} ({speaking.TimeOfEvent.ToLocalTime().ToString("dd.MM")}),"
            + $" {speaking.Venue.City}";
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

        foreach (var postMessage in sentPost)
        {
            AddMessage(postMessage);
        }
        lastPostMessages = sentPost;
    }
}
