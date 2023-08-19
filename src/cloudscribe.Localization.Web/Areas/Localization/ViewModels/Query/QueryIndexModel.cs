using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Localization.Models;

namespace cloudscribe.Localization.Web
{
    public class QueryIndexModel : BaseIndexModel<Query>
    {
        public int? DomainId { get; set; }

        public List<SelectListItem> Domains { get; internal set; } = null!;
    }
}
