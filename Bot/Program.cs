﻿using Bot.Forms;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TelegramBotBase.Args;
using TelegramBotBase.Builder;
using TelegramBotBase.Commands;
using TelegramBotBase.DependencyInjection;

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

bot.UploadBotCommands().Wait();

await bot.Start();

await app.RunAsync();

static async Task Bb_BotCommand(object sender, BotCommandEventArgs en)
{
    switch (en.Command)
    {
        case "/start":

            await en.Device.ActiveForm.NavigateTo<StartForm>();

            break;
    }
}
