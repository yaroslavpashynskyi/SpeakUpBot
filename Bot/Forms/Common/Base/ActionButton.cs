using TelegramBotBase.Form;

namespace Bot.Forms.Common.Base;

public class ActionButton : ButtonBase
{
    public Func<Task> Action { get; set; }

    public ActionButton(string text, string value, Func<Task> action)
        : base(text, value)
    {
        Action = action;
    }

    public async Task InvokeAction()
    {
        await Action.Invoke();
    }
}
