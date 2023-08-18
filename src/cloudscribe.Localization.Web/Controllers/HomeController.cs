using Arebis.Core.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cloudscribe.Localization.Web
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetStarted()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReloadFromSource([FromServices] ILocalizationResourceProvider resourceProvider)
        {
            resourceProvider.Refresh();

            return ForwardToAction("Index", target: "_self");
        }
    }
}
