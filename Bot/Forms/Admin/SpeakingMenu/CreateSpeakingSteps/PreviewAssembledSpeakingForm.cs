using Application.Speakings.Commands.CreateSpeaking;

using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;

public class PreviewAssembledSpeakingForm : AutoCleanForm
{
    private CreateSpeakingCommand _speakingData;

    public PreviewAssembledSpeakingForm()
    {
        Init += ListForm_Init;
    }

    private async Task ListForm_Init(object sender, InitEventArgs e)
    {
        _speakingData = (CreateSpeakingCommand)e.Args[0];
    }

    public override async Task Load(MessageResult message)
    {
        await Device.Send("Good");
    }
}
