using Application.Users.Commands.CreateUser;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Form;
using TelegramBotBase.DependencyInjection;
using Domain.Enums;
using Bot.Extensions;

namespace Bot.Forms.Common.UserRegistration.Steps;

public class GetEnglishLevelForm : AutoCleanForm
{
    public CreateUserCommand UserData { get; set; } = default!;

    public GetEnglishLevelForm()
    {
        Init += EnglishLevel_Init;
    }

    private Task EnglishLevel_Init(object sender, InitEventArgs e)
    {
        UserData = (CreateUserCommand)e.Args[0];
        return Task.CompletedTask;
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        Enum.TryParse(message.RawData, out EnglishLevel selectedLevel);
        if (selectedLevel == 0)
            return;

        UserData.EnglishLevel = selectedLevel;
    }

    public override async Task Render(MessageResult message)
    {
        if (UserData.EnglishLevel == 0)
        {
            var levels = Enum.GetValues(typeof(EnglishLevel)).Cast<EnglishLevel>();

            var bf = new ButtonForm();
            foreach (var level in levels)
            {
                bf.AddButtonRow(new ButtonBase(level.GetDescription(), level.ToString()));
            }

            await Device.Send("Оберіть ваш рівень англійської", bf);
            return;
        }

        await this.NavigateTo<GetSourceForm>(UserData);
    }
}
