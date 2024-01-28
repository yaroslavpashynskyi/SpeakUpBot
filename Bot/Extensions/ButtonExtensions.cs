using TelegramBotBase.Form;

namespace Bot.Extensions;

public static class ButtonExtensions
{
    public static bool IsEqual(this ButtonBase current, ButtonBase other)
    {
        return current.Text == other.Text && current.Value == other.Value;
    }
}
