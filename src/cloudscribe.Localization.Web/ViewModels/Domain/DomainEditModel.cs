using System.ComponentModel.DataAnnotations;
using cloudscribe.Localization.Models;

namespace cloudscribe.Localization.Web
{
    public class DomainEditModel : BaseEditModel<Domain>
    {
        [Required]
        public string? Cultures { get; set; }
    }
}
