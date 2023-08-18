namespace cloudscribe.Localization.EFCore.Common
{
    public interface ILocalizationDbContextFactory
    {
        ILocalizationDbContext CreateContext();
    }
}