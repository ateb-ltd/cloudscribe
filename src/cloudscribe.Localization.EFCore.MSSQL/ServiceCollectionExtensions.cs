using cloudscribe.Localization.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cloudscribe.Localization.EFCore.MSSQL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizationEFStorageMSSQL(
            this IServiceCollection services,
            string connectionString,
            bool useSingletonLifetime = false,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            ICollection<int> transientSqlErrorNumbersToAdd = null
            )
        {
            services.AddDbContext<LocalizationDbContext>(options =>
                    options.UseSqlServer(connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

                            if (maxConnectionRetryCount > 0)
                            {
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: maxConnectionRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(maxConnectionRetryDelaySeconds),
                                    errorNumbersToAdd: transientSqlErrorNumbersToAdd);
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
