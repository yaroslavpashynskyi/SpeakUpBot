using Bot.Forms.Common.UserRegistration.Steps;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.UserRegistration;

public class StartUserRegistrationForm : AutoCleanForm
{
    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        switch (message.RawData)
        {
            case "startRegistration":
                await this.NavigateTo<GetPhoneForm>();
                break;
        }
    }

    public override async Task Render(MessageResult message)
    {
        var bf = new ButtonForm();
        bf.AddButtonRow(new ButtonBase("Надати номер телефону", "startRegistration"));

        await Device.Send(
            "Вітаємо в боті Розмовного клубу SpeakUp!\nНаразі,"
                + " ви не зареєстровані.\nДля початку, надайте, будь ласка, ваш номер телефону.",
            bf
        );
    }
}
