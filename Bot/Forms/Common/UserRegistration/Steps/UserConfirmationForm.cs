using Application.Users.Commands.CreateUser;

using MediatR;

using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.UserRegistration.Steps;

public class UserConfirmationForm : AutoCleanForm
{
    private readonly IMediator _mediator;
    public CreateUserCommand UserData { get; set; } = default!;

    public UserConfirmationForm(IMediator mediator)
    {
        Init += Confirmation_Init;
        _mediator = mediator;
    }

    private Task Confirmation_Init(object sender, InitEventArgs e)
    {
        UserData = (CreateUserCommand)e.Args[0];
        return Task.CompletedTask;
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();
        switch (message.RawData)
        {
            case "confirm":
                UserData.TelegramId = Device.DeviceId;
                await _mediator.Send(UserData);
                break;
            case "startOver":
                await this.NavigateTo<StartUserRegistrationForm>();
                break;
            default:
                return;
        }
    }

    public override async Task Render(MessageResult message)
    {
        if (UserData.TelegramId == 0)
        {
            var bf = new ButtonForm();

            bf.AddButtonRow(new ButtonBase("Так, підтверджую", "confirm"));
            bf.AddButtonRow(new ButtonBase("Ні, почати реєстрацію знову", "startOver"));

            var userDataMessage =
                $"Ваші особисті дані:\n"
                + $"Номер телефону: {UserData.PhoneNumber}\n"
                + $"Ім'я: {UserData.FirstName}\n"
                + $"Прізвище: {UserData.LastName}\n"
                + $"Рівень англійської: {UserData.EnglishLevel}\n"
                + $"Звідки про нас дізнались: {UserData.Source.Title}\n"
                + "Чи підтверджуєте ви ці дані?";
            await Device.Send(userDataMessage, bf);

            return;
        }

        await this.NavigateTo<StartForm>();
    }
}
