using Application.Common.Interfaces;

using Domain.Enums;

using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddTransient<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);

        dataSourceBuilder.MapEnum<EnglishLevel>();
        dataSourceBuilder.MapEnum<PaymentStatus>();
        dataSourceBuilder.MapEnum<Role>();

        var dataSource = dataSourceBuilder.Build();
        services.AddDbContext<ApplicationDbContext>(
            (sp, opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                opt.UseNpgsql(dataSource);
            },
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient
        );
        services.AddTransient<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>()
        );

        return services;
    }
}
