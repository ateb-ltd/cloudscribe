using cloudscribe.Localization.Models;

namespace cloudscribe.Localization.Web
{
    public class KeyEditModel : BaseEditModel<Key>
    {
        public string? ArgumentNames { get; set; }

        public List<KeyValue> Values { get; set; } = new();

        public string? SourceCulture { get; set; }

        public bool SaveAsCopy { get; set; }

        public Domain[]? Domains { get; internal set; }

        public bool HasTranslationService { get; internal set; }
        
    }
}
