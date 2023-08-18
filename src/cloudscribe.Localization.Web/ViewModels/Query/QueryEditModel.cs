using cloudscribe.Localization.Models;

namespace cloudscribe.Localization.Web
{
    public class QueryEditModel : BaseEditModel<Query>
    {
        public Domain[]? Domains { get; internal set; } = null!;

        public string[]? ConnectionNames { get; internal set; } = null!;
    }
}
