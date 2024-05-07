using System.Reflection;

using Application.Common.Behaviors;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        services.AddTransient(
            (cfg) => new TelegramBotClient(configuration["BotConfiguration:BotToken"]!)
        );
        return services;
    }
}
