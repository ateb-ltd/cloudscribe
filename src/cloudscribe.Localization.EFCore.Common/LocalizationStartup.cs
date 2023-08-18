using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace cloudscribe.Localization.EFCore.Common
{
    public static class LocalizationStartup
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider scopedServiceProvider)
        {
            var db = scopedServiceProvider.GetService<ILocalizationDbContext>();

            if (db != null)
                await db.Database.MigrateAsync();
        }
    }
}
