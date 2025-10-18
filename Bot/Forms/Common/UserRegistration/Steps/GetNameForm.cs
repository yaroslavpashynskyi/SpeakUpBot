using Application.Users.Commands.CreateUser;

using Telegram.Bot.Types.ReplyMarkups;

using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.UserRegistration.Steps;

public class GetNameForm : AutoCleanForm
{
    public CreateUserCommand UserData { get; set; } = default!;

    public GetNameForm()
    {
        Init += NameForm_Init;
    }

    private Task NameForm_Init(object sender, InitEventArgs e)
    {
        UserData = (CreateUserCommand)e.Args[0];
        return Task.CompletedTask;
    }

    public override async Task Load(MessageResult message)
    {
        if (message.MessageText.Trim() == "")
        {
            return;
        }
        if (message.MessageText.Length > 50)
        {
            await Device.Send("Ім'я та прізвище не може бути більше ніж 50 символів!");
            return;
        }
        if (UserData.Name == null)
        {
            UserData.Name = message.MessageText;
            return;
        }

    }

    public override async Task Render(MessageResult message)
    {
        if (UserData.Name == null)
        {
            await Device.Send("Введіть своє ім'я та прізвище", markup: new ReplyKeyboardRemove());
            return;
        }

        await this.NavigateTo<GetEnglishLevelForm>(UserData);
    }
}
