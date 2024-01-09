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
        if (UserData.FirstName == null)
        {
            UserData.FirstName = message.MessageText;
            return;
        }
        if (UserData.LastName == null)
        {
            UserData.LastName = message.MessageText;
            return;
        }
    }

    public override async Task Render(MessageResult message)
    {
        if (UserData.FirstName == null)
        {
            await Device.Send("Введіть своє справжнє ім'я", markup: new ReplyKeyboardRemove());
            return;
        }
        if (UserData.LastName == null)
        {
            await Device.Send("Введіть своє прізвище");
            return;
        }

        await this.NavigateTo<GetEnglishLevelForm>(UserData);
    }
}
