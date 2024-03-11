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
        var ms = await Device.Send(
            "👋Привіт!\r\n\r\nЛаскаво просимо в чат-боті англомовної розмовної "
                + "спільноти SpeakUp!\r\n\r\nЯ з радістю допоможу Вам:\r\n\r\n🚀 дізнатися про майбутні"
                + " івенти SpeakUp у Броварах і Києві\r\n✍️ зареєструватися на івент або відмінити запис\r\n"
                + "📞 звʼязатись з організаторами\r\n\r\n💡Щоб першим дізнаватись про новини, а також отримувати фото"
                + " з  заходів, слідкуйте за нашими соцмережами "
        );
        LeaveMessage(ms.MessageId);

        await Device.Send(
            "Вітаємо в боті Розмовного клубу SpeakUp!\nНаразі,"
                + " ви не зареєстровані.\nДля початку, надайте, будь ласка, ваш номер телефону.",
            bf
        );
    }
}
