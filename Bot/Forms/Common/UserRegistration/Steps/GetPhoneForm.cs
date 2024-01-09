using Application.Users.Commands.CreateUser;

using Telegram.Bot.Types;

using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.UserRegistration.Steps;

public class GetPhoneForm : AutoCleanForm
{
    public CreateUserCommand UserData { get; set; } = default!;

    public GetPhoneForm()
    {
        Init += PhoneNumber_Init;
    }

    private Task PhoneNumber_Init(object sender, InitEventArgs e)
    {
        UserData = new CreateUserCommand();
        return Task.CompletedTask;
    }

    public override Task SentData(DataResult message)
    {
        if (message.Contact is Contact contact)
        {
            UserData.PhoneNumber = contact.PhoneNumber;
        }
        return Task.CompletedTask;
    }

    public override async Task Render(MessageResult message)
    {
        if (UserData.PhoneNumber is null)
        {
            await Device.RequestContact(
                buttonText: "Поділитися номером",
                requestMessage: "Будь ласка, надайте ваш номер телефону"
            );
            return;
        }

        await this.NavigateTo<GetNameForm>(UserData);
    }
}
