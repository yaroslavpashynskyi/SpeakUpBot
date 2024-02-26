using Bot.Forms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using TelegramBotBase;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;
using TelegramBotBase.Sessions;

namespace Bot.Extensions;

public static class BotExtenstions
{
    public static void AddHandlers(this BotBase bot, IServiceProvider services)
    {
        bot.SetSetting(ESettings.LogAllMessages, true);

        bot.BotCommand += Bb_BotCommand;
        bot.Exception += Bot_Exception;
        bot.Message += Bot_Message;

        void Bot_Message(object? sender, MessageIncomeEventArgs e)
        {
            var logger = services.GetRequiredService<ILogger>();
            logger.Information(
                "User {TelegramUserId} sent {Message} in {Form}",
                e.DeviceId,
                e.Message.RawData ?? e.Message.MessageText,
                e.Device.ActiveForm
            );
        }

        void Bot_Exception(object? sender, SystemExceptionEventArgs e)
        {
            var env = services.GetRequiredService<IHostEnvironment>();

            if (env.IsDevelopment())
                throw e.Error;
            else
            {
                var logger = services.GetRequiredService<ILogger>();
                logger.Error("Error occurred {Error} {DeviceId}", e.Error, e.DeviceId);
            }
        }

        bot.UploadBotCommands().Wait();

        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        async void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            foreach (var session in bot.Sessions.SessionList.Values)
                await ClearSessions(session);
        }

        static async Task Bb_BotCommand(object sender, BotCommandEventArgs en)
        {
            await en.Device.DeleteMessage(en.OriginalMessage);
            switch (en.Command)
            {
                case "/start":
                    var activeForm = await ClearSessions(en.Device);
                    await activeForm.NavigateTo<StartForm>();
                    break;
            }
        }

        static async Task<FormBase> ClearSessions(DeviceSession session)
        {
            var activeForm = session.ActiveForm;
            if (activeForm.IsAutoCleanForm())
                await ((AutoCleanForm)activeForm).MessageCleanup();
            return activeForm;
        }
    }
}
