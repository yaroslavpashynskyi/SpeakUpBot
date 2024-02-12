using System.Globalization;
using System.Text;
using Telegram.Bot.Types.Enums;

using Domain.Entities;

using Telegram.Bot.Types;

using TelegramBotBase.Sessions;
using Telegram.Bot;
using System.ComponentModel;

namespace Bot.Extensions;

public static class SpeakingExtensions
{
    public static async Task<Message[]?> SendSpeakingPost(
        this DeviceSession device,
        Speaking speaking
    )
    {
        CultureInfo cultureInfo = new CultureInfo("uk-UA");
        StringBuilder post = new StringBuilder();
        string newLine = Environment.NewLine;
        var localTimeOfEvent = speaking.TimeOfEvent.ToLocalTime();

        if (!string.IsNullOrWhiteSpace(speaking.Intro))
        {
            post.Append(speaking.Intro);
            post.AppendLine(newLine);
        }
        post.AppendFormat(
            "📅{0}{1}",
            localTimeOfEvent.ToString("d MMMM (dddd)", cultureInfo),
            newLine
        );
        var timeStart = localTimeOfEvent.ToShortTimeString();
        var timeEnd = localTimeOfEvent.AddMinutes(speaking.DurationMinutes).ToShortTimeString();
        post.AppendFormat("🕔{0} - {1}{2}", timeStart, timeEnd, newLine);
        post.AppendFormat(
            "📍<a href=\"{0}\">{1}</a>",
            speaking.Venue.InstagramUrl,
            speaking.Venue.Name
        );
        post.AppendFormat(" (<a href=\"{0}\">Локація</a>){1}", speaking.Venue.LocationUrl, newLine);
        post.AppendFormat("💵{0} грн{1}", speaking.Price, newLine);
        post.Append(newLine);
        post.Append(speaking.Description);

        switch (speaking.Photos.Count)
        {
            case 1:
                try
                {
                    return new[]
                    {
                        await device.Client.TelegramClient.SendPhotoAsync(
                            device.DeviceId,
                            InputFile.FromFileId(speaking.Photos.First().FileId),
                            caption: post.ToString(),
                            parseMode: ParseMode.Html
                        )
                    };
                }
                catch
                {
                    return null;
                }
            case > 1:
            {
                IEnumerable<InputMediaPhoto> photos = speaking.Photos
                    .OrderBy(p => p.OrdinalNumber)
                    .Select(p => new InputMediaPhoto(InputFile.FromFileId(p.FileId)))
                    .ToList();

                var first = photos.First();
                first.Caption = post.ToString();
                first.ParseMode = ParseMode.Html;

                Message[] result;
                try
                {
                    result = await device.Client.TelegramClient.SendMediaGroupAsync(
                        device.DeviceId,
                        photos
                    );
                }
                catch (Exception ex)
                {
                    return null;
                }

                return result;
            }

            default:
                return new[] { await device.Send("Невизначена кількість фото") };
        }
    }
}
