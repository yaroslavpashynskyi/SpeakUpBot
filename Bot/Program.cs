using Bot.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TelegramBotBase.Builder;

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
    .NoCommands()
    .NoSerialization()
    .DefaultLanguage()
    .Build();

await bot.Start();

await app.RunAsync();
