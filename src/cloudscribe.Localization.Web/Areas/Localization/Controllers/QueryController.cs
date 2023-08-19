using cloudscribe.Localization.EFCore.Common;
using cloudscribe.Localization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace cloudscribe.Localization.Web
{
    [Route("Localization/Query")]
    public class QueryController : BaseController
    {
        #region Construction

        private readonly ILocalizationDbContext context;
        private readonly ILogger logger;

        public QueryController(ILocalizationDbContext context, ILogger<KeyController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        #endregion

        #region Index

        [HttpGet]
        public async Task<IActionResult> Index(QueryIndexModel model)
        {
            var count = await context.LocalizeQueries
                .Where(i => i.DomainId == model.DomainId || model.DomainId == null)
                .Where(i => i.Name.Contains(model.Query ?? ""))
                .CountAsync();
            model.MaxPage = (count + model.PageSize - 1) / model.PageSize;
            model.Items = await context.LocalizeQueries
                .Include(i => i.Domain)
                .Where(i => i.DomainId == model.DomainId || model.DomainId == null)
                .Where(i => i.Name.Contains(model.Query ?? ""))
                .OrderBy(model.Order ?? "Name ASC")
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToArrayAsync();
            model.Domains = await context.LocalizeDomains
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem() { Value = d.Id.ToString(), Text = d.Name, Selected = (model.DomainId == d.Id) })
                .ToListAsync();

            return View("Index", model);
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> New(int? domainId)
        {
            var model = new QueryEditModel();
            model.Item = new Query() { DomainId = domainId ?? 0 };
            return await EditView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await context.LocalizeQueries
                .Include(q => q.Domain)
                .SingleOrDefaultAsync(q => q.Id == id);
            if (item == null) return new NotFoundResult();

            var model = new QueryEditModel() { Item = item };
            return await EditView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(int id, QueryEditModel model, bool apply = false)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.LocalizeQueries.Update(model.Item);
                    await context.SaveChangesAsync();
                    if (!apply) return Back(false);
                    else
                    {
                        ModelState.Clear();
                        model.HasChanges = false;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error saving query {0}", id);
                    ModelState.AddModelError("", "An unexpected error occured.");
                    ViewBag.Exception = ex;
                }
            }
            else
            {
                SetToastrMessage("error", "Failed to save the query.<br/>See validation messages for more information.");
            }

            Response.Headers.Add("X-Sircl-History-Replace", Url.Action("Edit", new { id = model.Item.Id }));
            return await EditView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, QueryEditModel model)
        {
            try
            {
                var item = await context.LocalizeQueries.FindAsync(id);
                if (item != null)
                {
                    context.LocalizeQueries.Remove(item);
                    await context.SaveChangesAsync();
                }
                return Back(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error deleting query {0}", id);
                ModelState.AddModelError("", "An unexpected error occured.");
                ViewBag.Exception = ex;
            }

            return await EditView(model);
        }

        private async Task<IActionResult> EditView(QueryEditModel model)
        {
            model.Domains = await context.LocalizeDomains.OrderBy(d => d.Name).ToArrayAsync();
            model.ConnectionNames = await context.LocalizeQueries.Select(q => q.ConnectionName).Distinct().OrderBy(n => n).ToArrayAsync();

            return View("Edit", model);
        }

        #endregion
    }
}
