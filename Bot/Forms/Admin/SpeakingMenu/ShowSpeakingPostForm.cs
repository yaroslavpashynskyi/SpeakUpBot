using Bot.Extensions;

using Domain.Entities;

using Telegram.Bot.Types;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu;

public class ShowSpeakingPostForm : AutoCleanForm
{
    private Speaking? _speaking;

    public ShowSpeakingPostForm()
    {
        DeleteMode = EDeleteMode.OnLeavingForm;
        DeleteSide = EDeleteSide.Both;
        Init += ShowSpeakingPostForm_Init;
    }

    private Task ShowSpeakingPostForm_Init(object sender, TelegramBotBase.Args.InitEventArgs e)
    {
        if (e.Args.Length > 0)
        {
            try
            {
                _speaking = (Speaking)e.Args[0];
            }
            catch
            {
                throw;
            }
        }
        return Task.CompletedTask;
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        if (message.RawData == "back")
            await this.NavigateTo<SpeakingListForm>();
    }

    public override async Task Load(MessageResult message)
    {
        Message[]? sentPost = await Device.SendSpeakingPost(_speaking!);

        if (sentPost == null)
            return;

        foreach (var postMessage in sentPost)
        {
            AddMessage(postMessage);
        }

        var bf = new ButtonForm();
        bf.AddButtonRow("Назад", "back");

        await Device.Send("Повернутись назад", bf);
    }
}
