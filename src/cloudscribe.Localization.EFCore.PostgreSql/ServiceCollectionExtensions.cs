using cloudscribe.Localization.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cloudscribe.Localization.EFCore.PostgreSql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizationEFStoragePostgreSql(
            this IServiceCollection services,
            string connectionString,
            bool useSingletonLifetime = false,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            ICollection<string> transientErrorCodesToAdd = null!
            )
        {
            services.AddDbContext<LocalizationDbContext>(options =>
                    options.UseNpgsql(connectionString,
                        npgsqlOptionsAction: sqlOptions =>
                        {
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                            if (maxConnectionRetryCount > 0)
                            {
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: maxConnectionRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(maxConnectionRetryDelaySeconds),
                                    errorCodesToAdd: transientErrorCodesToAdd);
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
