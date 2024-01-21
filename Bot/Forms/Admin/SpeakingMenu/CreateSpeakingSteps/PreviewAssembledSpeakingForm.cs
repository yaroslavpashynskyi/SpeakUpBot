using Application.Extensions;
using Bot.Extensions;
using Application.Speakings.Commands.CreateSpeaking;
using Telegram.Bot.Types;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Form;
using MediatR;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;

namespace Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;

public class PreviewAssembledSpeakingForm : AutoCleanForm
{
    private readonly IMediator _mediator;
    private CreateSpeakingCommand _speakingData = new();
    private readonly ButtonBase _startOver = new("Почати знову", "startOver");
    private readonly ButtonBase _sumbit = new("Підтвердити", "confirm");

    public PreviewAssembledSpeakingForm(IMediator mediator)
    {
        Init += ListForm_Init;
        _mediator = mediator;
    }

    private Task ListForm_Init(object sender, InitEventArgs e)
    {
        _speakingData = (CreateSpeakingCommand)e.Args[0];
        return Task.CompletedTask;
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        if (message.RawData == _sumbit.Value)
        {
            await _mediator.Send(_speakingData);
            await this.NavigateTo<SpeakingMenuForm>();
        }
        else if (message.RawData == _startOver.Value)
            await this.NavigateTo<StartCreatingSpeakingForm>();

        return;
    }

    public override async Task Render(MessageResult message)
    {
        var speaking = _speakingData.CreateCommandToSpeaking();
        Message[]? result = await Device.SendSpeakingPost(speaking);

        var bf = new ButtonForm();
        if (result == null)
        {
            bf.AddButtonRow(_startOver);
            await Device.Send("Занадто багато символів в пості.", bf);
            return;
        }
        foreach (var postMessage in result)
        {
            AddMessage(postMessage);
        }

        bf.AddButtonRow(_sumbit, _startOver);
        await Device.Send("Чи пітверджуєте ви створення посту?", bf);
        return;
    }
}
