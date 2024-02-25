using System.Reflection;

using Application.Common.Behaviors;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        services.AddTransient(
            (cfg) =>
                new TelegramBotClient(
                    cfg.GetRequiredService<IConfiguration>()["BotConfiguration:BotToken"]!
                )
        );
        return services;
    }
}
