using cloudscribe.Localization.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cloudscribe.Localization.EFCore.SQLite
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizationEFStorageSQLite(
            this IServiceCollection services,
            string connectionString,
            bool useSingletonLifetime = false,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            int commandTimeout = 30
            )
        {
            services.AddDbContext<LocalizationDbContext>(options =>
                    options.UseSqlite(connectionString,
                        sqliteOptionsAction: sqlOptions =>
                        {
                            if (maxConnectionRetryCount > 0)
                            {
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                                sqlOptions.CommandTimeout(commandTimeout);
                            }

                        }),
                        optionsLifetime: ServiceLifetime.Singleton
                    );

            services.AddScoped<ILocalizationDbContext, LocalizationDbContext>();
            services.AddSingleton<ILocalizationDbContextFactory, LocalizationDbContextFactory>();

            return services;
        }
    }
}
