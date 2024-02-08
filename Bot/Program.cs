using Bot.Forms;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TelegramBotBase.Args;
using TelegramBotBase.Builder;
using TelegramBotBase.Commands;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;
using TelegramBotBase.Sessions;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(
    (context, services) =>
    {
        services.AddInfrastructureServices(context.Configuration);
        services.AddApplicationServices();
    }
);
var app = builder.Build();

var bot = BotBaseBuilder
    .Create()
    .WithAPIKey(app.Services.GetRequiredService<IConfiguration>()["BotConfiguration:BotToken"])
    .DefaultMessageLoop()
    .WithServiceProvider<StartForm>(app.Services)
    .NoProxy()
    .CustomCommands(a =>
    {
        a.Start("Початок роботи");
    })
    .NoSerialization()
    .DefaultLanguage()
    .Build();

bot.BotCommand += Bb_BotCommand;
bot.Exception += Bot_Exception;

void Bot_Exception(object? sender, SystemExceptionEventArgs e)
{
    var env = app.Services.GetRequiredService<IHostEnvironment>();

    if (env.IsDevelopment())
        throw e.Error;
}

bot.UploadBotCommands().Wait();

await bot.Start();

await app.RunAsync();
AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
async void CurrentDomain_ProcessExit(object? sender, EventArgs e)
{
    foreach (var session in bot.Sessions.SessionList.Values)
        await ClearSessions(session);
}

static async Task Bb_BotCommand(object sender, BotCommandEventArgs en)
{
    switch (en.Command)
    {
        case "/start":

            await en.Device.ActiveForm.NavigateTo<StartForm>();

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
