// ProductWarrantyController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.ProductWarranty.Models;
using Nop.Plugin.Misc.ProductWarranty.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.ProductWarranty.Controllers
{
    public class ProductWarrantyController : BasePluginController
    {
        private readonly IWarrantyService _warrantyService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public ProductWarrantyController(
            IWarrantyService warrantyService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _warrantyService = warrantyService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        /// <summary>
        /// Gets warranty details for a product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>PartialView with warranty information</returns>
        public async Task<IActionResult> ProductWarrantyDetails(int productId)
        {
            // Get settings
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var settings = await _settingService.LoadSettingAsync<ProductWarrantySettings>(storeId);

            // Check if the plugin is enabled
            if (!settings.Enabled)
                return Json(new { success = false });

            // Get warranty information for this product
            var mappings = await _warrantyService.GetProductWarrantyMappingsByProductIdAsync(productId);
            if (!mappings.Any())
                return Json(new { success = false });

            // Prepare model
            var model = new PublicWarrantyModel();

            foreach (var mapping in mappings)
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
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                html = await this.RenderPartialViewToStringAsync("~/Views/WarrantyInfo/Default.cshtml", model)
            });
        }
    }
}