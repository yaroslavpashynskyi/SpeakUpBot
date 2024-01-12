namespace Bot.Utils;

public static class FormatVenue
{
    public static string Format(string name, string city, Uri location, Uri instagram)
    {
        return $"Назва закладу: {name}\n"
            + $"Місто: {city}\n"
            + $"Точне місцезнаходження: {location}\n"
            + $"Посилання на Instagram: {instagram}";
    }
}
