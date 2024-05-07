using Bot;
using Bot.Extensions;
using Bot.Forms;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using Telegram.Bot;

using TelegramBotBase.Builder;
using TelegramBotBase.Commands;

var builder = Host.CreateDefaultBuilder(args);

builder.UseSerilog(
    (hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
);

builder.ConfigureServices(
    (context, services) =>
    {
        services.AddInfrastructureServices(context.Configuration);
        services.AddApplicationServices(context.Configuration);
    }
);
var app = builder.Build();

var bot = BotBaseBuilder
    .Create()
    .WithAPIKey(app.Services.GetRequiredService<IConfiguration>()["BotConfiguration:BotToken"])
    .CustomMessageLoop<CustomMessageLoop>()
    .WithServiceProvider<StartForm>(app.Services)
    .WithBotClient(app.Services.GetRequiredService<TelegramBotClient>())
    .CustomCommands(a => a.Start("Головне меню"))
    .NoSerialization()
    .DefaultLanguage()
    .Build();

var context = app.Services.GetRequiredService<ApplicationDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    context.Database.Migrate();
}

bot.AddHandlers(app.Services);
await bot.Start();

await app.RunAsync();
