using Application.Speakings.Commands.CreateSpeaking;

using Domain.Entities;

using MediatR;

namespace Application.Extensions;

public static class SpeakingExtensions
{
    public static Speaking CreateCommandToSpeaking(this CreateSpeakingCommand command)
    {
        var speaking = new Speaking
        {
            Title = command.Title,
            Intro = command.Intro,
            Description = command.Description,
            Price = command.Price,
            Seats = command.Seats,
            TimeOfEvent = command.DateOfEvent.ToDateTime(command.TimeOfEvent).ToUniversalTime(),
            DurationMinutes = command.DurationMinutes,
            Venue = command.Venue,
        };

        foreach (var photo in command.Photos)
        {
            speaking.Photos.Add(
                new Photo
                {
                    FileId = photo.FileId,
                    OrdinalNumber = photo.OrdinalNumber,
                    Speaking = speaking
                }
            );
        }
        return speaking;
    }

    public static string GetName(this Speaking speaking) =>
        $"{speaking.Title} ({DateTime.Now.ToString("dd/MM")})";
}
