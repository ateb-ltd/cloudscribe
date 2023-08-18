using cloudscribe.Localization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace cloudscribe.Localization.EFCore.Common
{
    public interface ILocalizationDbContext : IDisposable
    {
        public DbSet<Domain> LocalizeDomains { get; set; }
        
        public DbSet<Key> LocalizeKeys { get; set; }

        public DbSet<KeyValue> LocalizeKeyValues { get; set; }

        public DbSet<Query> LocalizeQueries { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        // void Remove(object entity);

        // void Update(object entity);
        
        // int SaveChanges ();


        DatabaseFacade Database { get; }
    }
}