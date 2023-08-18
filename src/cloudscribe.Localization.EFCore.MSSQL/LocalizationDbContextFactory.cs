using cloudscribe.Localization.EFCore.Common;
using Microsoft.EntityFrameworkCore;

namespace cloudscribe.Localization.EFCore.MSSQL
{
    public class LocalizationDbContextFactory : ILocalizationDbContextFactory
    {
        public LocalizationDbContextFactory(DbContextOptions<LocalizationDbContext> options)
        {
            _options = options;
        }

        private readonly DbContextOptions<LocalizationDbContext> _options;

        public ILocalizationDbContext CreateContext()
        {
            return new LocalizationDbContext(_options);
        }
    }
}
