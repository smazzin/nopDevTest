using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.ProductWarranty.Models;
using Nop.Plugin.Misc.ProductWarranty.Services;
using Nop.Plugin.Misc.ProductWarranty;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

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
    /// <param name="widgetZone">Widget zone where it's displayed</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>View component result</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
    {
        // Only process if we're in the product details area
        if (widgetZone != PublicWidgetZones.ProductDetailsEssentialBottom)
            return Content("");

        // Get product ID from route data
        //if (!Url.ActionContext.RouteData.Values.ContainsKey("productId"))
        //    return Content("");

        //var productIdStr = Url.ActionContext.RouteData.Values["productId"]?.ToString();
        //if (string.IsNullOrEmpty(productIdStr) || !int.TryParse(productIdStr, out var productId) || productId == 0)
        //    return Content("");

        // Get settings
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var settings = await _settingService.LoadSettingAsync<ProductWarrantySettings>(storeId);

        // Check if the plugin is enabled and configured to display on product page
        if (!settings.Enabled || !settings.DisplayWarrantyOnProductPage)
            return Content("");

        var productId = Convert.ToInt32(Url.ActionContext.RouteData.Values["productId"]);

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

        // Make sure to use the correct path to your view
        return View("~/Plugins/Misc.ProductWarranty/Views/WarrantyInfo/Default.cshtml", model);
    }
}