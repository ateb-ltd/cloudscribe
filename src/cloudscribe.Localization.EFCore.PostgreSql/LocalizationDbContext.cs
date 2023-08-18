using cloudscribe.EFCore.PostgreSql.Conventions;
using cloudscribe.Localization.EFCore.Common;
using cloudscribe.Localization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cloudscribe.Localization.EFCore.PostgreSql
{
    public class LocalizationDbContext : DbContext, ILocalizationDbContext
    {
        private static readonly char[] StringSeparator = new char[] { ',' };

        public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Domain>()
                .ToTable("csl_localization_domains")
                .Property(e => e.Cultures)
                .HasConversion(
                    a => (a == null || a.Length == 0) ? null : String.Join(StringSeparator[0], a),
                    s => (s == null) ? null : s.Split(StringSeparator),
                    new ValueComparer<string[]>(
                        (a1, a2) => a1!.SequenceEqual(a2!),
                        a => a.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        a => a.ToArray()
                    )
                );

            modelBuilder
                .Entity<Key>()
                .ToTable("csl_localization_keys")
                .Property(e => e.ArgumentNames)
                .HasConversion(
                    a => (a == null || a.Length == 0) ? null : String.Join(StringSeparator[0], a),
                    s => (s == null) ? null : s.Split(StringSeparator),
                    new ValueComparer<string[]>(
                        (a1, a2) => a1!.SequenceEqual(a2!),
                        a => a.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        a => a.ToArray()
                    )
                );

            modelBuilder
                .Entity<Key>()
                .ToTable("csl_localization_keys")
                .Property(e => e.ValuesToReview)
                .HasConversion(
                    a => (a == null || a.Length == 0) ? null : String.Join(StringSeparator[0], a),
                    s => (s == null) ? null : s.Split(StringSeparator),
                    new ValueComparer<string[]>(
                        (a1, a2) => a1!.SequenceEqual(a2!),
                        a => a.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        a => a.ToArray()
                    )
                );

            modelBuilder
                .Entity<KeyValue>()
                .ToTable("csl_localization_keyvalue");

            modelBuilder
                .Entity<Query>()
                .ToTable("csl_localization_query");

            modelBuilder.ApplySnakeCaseConventions();
        }

        public DbSet<Domain> LocalizeDomains { get; set; } = null!;
        
        public DbSet<Key> LocalizeKeys { get; set; } = null!;

        public DbSet<KeyValue> LocalizeKeyValues { get; set; } = null!;

        public DbSet<Query> LocalizeQueries { get; set; } = null!;

    }
}