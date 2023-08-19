using cloudscribe.Localization.EFCore.Common;
using cloudscribe.Localization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Mime;

using Microsoft.EntityFrameworkCore;

namespace cloudscribe.Localization.Web
{
    [Route("Localization/Domain")]
    public class DomainController : BaseController
    {
        #region Construction

        private readonly ILocalizationDbContext context;
        private readonly ILogger logger;

        public DomainController(ILocalizationDbContext context, ILogger<DomainController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        #endregion

        #region Index

        [HttpGet]
        public IActionResult Index(DomainIndexModel model)
        {
            model.MaxPage = 1;
            model.Items = context.LocalizeDomains
                .OrderBy(i => i.Name).ThenBy(i => i.Id)
                .ToArray();

            return View("Index", model);
        }

        #endregion

        #region Edit

        public IActionResult New([FromServices] IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            var model = new DomainEditModel
            {
                Item = new Domain()
            };
            if (requestLocalizationOptions.Value.SupportedUICultures != null)
                model.Cultures = String.Join(',', requestLocalizationOptions.Value.SupportedUICultures.Select(c => c.TwoLetterISOLanguageName).Distinct());

            return EditView(model);
        }

        public IActionResult Edit(int id)
        {
            var item = context.LocalizeDomains.Find(id);
            if (item == null) return new NotFoundResult();

            var model = new DomainEditModel() { Item = item };
            model.Cultures = String.Join(", ", model.Item.Cultures ?? Array.Empty<string>());

            return EditView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int id, DomainEditModel model)
        {
            // Validate cultures:
            if (model.Cultures != null)
            {
                var cultures = model.Cultures.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
                if (cultures.Distinct().Count() != cultures.Length)
                {
                    ModelState.AddModelError("Cultures", "Value should not contain duplicates!");
                }
                else if (cultures.Length == 0)
                {
                    ModelState.AddModelError("Cultures", "Value is required!");
                }
                else
                {
                    model.Item.Cultures = cultures;
                }
            }
            else
            {
                ModelState.AddModelError("Cultures", "Value is required!");
            }

            // Validate modelstate and save:
            if (ModelState.IsValid)
            {
                try
                {
                    context.LocalizeDomains.Update(model.Item);
                    await context.SaveChangesAsync();
                    return this.Close(true);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error saving domain {domainid}", id);
                    ModelState.AddModelError("", "An unexpected error occured.");
                    ViewBag.Exception = ex;
                }
            }

            return EditView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, DomainEditModel model)
        {
            try
            {
                var item = context.LocalizeDomains.Find(id);
                if (item != null)
                {
                    context.LocalizeDomains.Remove(item);
                    await context.SaveChangesAsync();
                }
                return this.Close(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error deleting domain {domainid}", id);
                ModelState.AddModelError("", "An unexpected error occured.");
                ViewBag.Exception = ex;
            }

            return EditView(model);
        }

        private IActionResult EditView(DomainEditModel model)
        {
            return View("Edit", model);
        }

        #endregion

        #region Import

        [HttpPost]
        public Task<IActionResult> Import(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var domain = JsonSerializer.Deserialize<Domain>(stream);
                if (domain != null)
                {
                    context.LocalizeDomains.Add(domain);
                    context.SaveChangesAsync();
                    SetToastrMessage("success", $"Domain {domain.Name} imported with Id {domain.Id}.");
                }
            }

            var result = Index(new DomainIndexModel());
            return Task.FromResult(result);
        }

        #endregion

        #region Export

        public IActionResult Export(int id)
        {
            var domain = context.LocalizeDomains
                .Include(d => d.Keys!).ThenInclude(k => k.Values)
                .Include(d => d.Queries)
                .Single(d => d.Id == id);

            var json = JsonSerializer.Serialize(domain);
            return this.Content(json, MediaTypeNames.Application.Json);
        }

        #endregion
    }
}
