// WarrantyInfoViewComponent.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.ProductWarranty.Models;
using Nop.Plugin.Misc.ProductWarranty.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.ProductWarranty.Components
{
    /// <summary>
    /// Represents the view component to display warranty information on the product page
    /// </summary>
    [ViewComponent(Name = "WarrantyInfo")]
    public class WarrantyInfoViewComponent : NopViewComponent
    {
        private readonly IWarrantyService _warrantyService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WarrantyInfoViewComponent(
            IWarrantyService warrantyService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _warrantyService = warrantyService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            // Get settings
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var settings = await _settingService.LoadSettingAsync<ProductWarrantySettings>(storeId);

            // Check if the plugin is enabled and configured to display on product page
            if (!settings.Enabled || !settings.DisplayWarrantyOnProductPage)
                return Content("");

            // Get warranty information for this product
            var mappings = await _warrantyService.GetProductWarrantyMappingsByProductIdAsync(productId);
            if (!mappings.Any())
                return Content("");

            // Prepare model
            var model = new PublicWarrantyModel();

            foreach (var mapping in mappings.Where(m => m.IsActive))
            {
                var category = await _warrantyService.GetWarrantyCategoryByIdAsync(mapping.WarrantyCategoryId);
                if (category == null || !category.Published)
                    continue;

                model.WarrantyItems.Add(new PublicWarrantyModel.WarrantyItemModel
                {
                    CategoryId = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    DurationMonths = category.DurationMonths,
                    Notes = mapping.Notes
                });
            }

            if (!model.WarrantyItems.Any())
                return Content("");

            return View("~/Views/WarrantyInfo/Default.cshtml", model);
        }
    }
}