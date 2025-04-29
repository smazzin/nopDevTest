using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models;
using Nop.Plugin.Misc.ProductWarranty.Domain;
using Nop.Plugin.Misc.ProductWarranty.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Controllers
{
    [Area(AreaNames.ADMIN)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class WarrantyController : BasePluginController
    {
        #region Fields

        private readonly IWarrantyService _warrantyService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public WarrantyController(
            IWarrantyService warrantyService,
            ISettingService settingService,
            IStoreContext storeContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IProductService productService)
        {
            _warrantyService = warrantyService;
            _settingService = settingService;
            _storeContext = storeContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _productService = productService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare available warranty categories for product
        /// </summary>
        /// <returns>List of available warranty categories</returns>
        protected virtual async Task<IList<SelectListItem>> PrepareWarrantyCategoriesAsync()
        {
            var availableItems = new List<SelectListItem>();
            var categories = await _warrantyService.GetAllWarrantyCategoriesAsync(true);
            
            foreach (var category in categories)
            {
                availableItems.Add(new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString()
                });
            }

            return availableItems;
        }

        /// <summary>
        /// Prepare available products for product warranty mapping
        /// </summary>
        /// <returns>List of available products</returns>
        protected virtual async Task<IList<SelectListItem>> PrepareProductsAsync()
        {
            var availableItems = new List<SelectListItem>();
            var products = await _productService.SearchProductsAsync(
                showHidden: true, 
                pageSize: int.MaxValue);
            
            foreach (var product in products)
            {
                availableItems.Add(new SelectListItem
                {
                    Text = product.Name,
                    Value = product.Id.ToString()
                });
            }

            return availableItems;
        }

        #endregion

        #region Configuration

        public async Task<IActionResult> Configure()
        {
            // Get settings for active store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<ProductWarrantySettings>(storeScope);

            var model = new ConfigurationModel
            {
                Enabled = settings.Enabled,
                DisplayWarrantyOnProductPage = settings.DisplayWarrantyOnProductPage,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.Enabled, storeScope);
                model.DisplayWarrantyOnProductPage_OverrideForStore = await _settingService.SettingExistsAsync(settings, x => x.DisplayWarrantyOnProductPage, storeScope);
            }

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            // Get settings for active store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<ProductWarrantySettings>(storeScope);

            settings.Enabled = model.Enabled;
            settings.DisplayWarrantyOnProductPage = model.DisplayWarrantyOnProductPage;

            // Save settings
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.DisplayWarrantyOnProductPage, model.DisplayWarrantyOnProductPage_OverrideForStore, storeScope, false);
            
            // Clear cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            
            return await Configure();
        }

        #endregion

        #region Warranty Categories

        public async Task<IActionResult> Categories()
        {

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/Categories.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CategoryList()
        {
            // Get draw value from the request if available
            int draw = 1;
            if (Request.Form.ContainsKey("draw"))
                int.TryParse(Request.Form["draw"].FirstOrDefault(), out draw);

            var categories = await _warrantyService.GetAllWarrantyCategoriesAsync(true);

            // Debug to output
            System.Diagnostics.Debug.WriteLine($"Found {categories.Count} warranty categories");

            var model = categories.Select(x => new WarrantyCategoryModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description ?? "", // Ensure no null values
                DurationMonths = x.DurationMonths,
                DisplayOrder = x.DisplayOrder,
                Published = x.Published,
                CreatedOn = x.CreatedOnUtc,
                UpdatedOn = x.UpdatedOnUtc
            }).ToList();

            // Return in the format expected by DataTables
            return Json(model);
        }

        public async Task<IActionResult> CreateCategory()
        {
            var model = new WarrantyCategoryModel
            {
                Published = true,
                DisplayOrder = 1
            };

            // Use the full path to the view
            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/CreateCategory.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> CreateCategory(WarrantyCategoryModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var category = new WarrantyCategoryRecord
                {
                    Name = model.Name,
                    Description = model.Description,
                    DurationMonths = model.DurationMonths,
                    DisplayOrder = model.DisplayOrder,
                    Published = model.Published
                };

                await _warrantyService.InsertWarrantyCategoryAsync(category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyCategory.Created"));

                if (continueEditing)
                    return RedirectToAction("EditCategory", new { id = category.Id });

                return RedirectToAction("Categories");
            }

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/CreateCategory.cshtml", model);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _warrantyService.GetWarrantyCategoryByIdAsync(id);

            var model = new WarrantyCategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DurationMonths = category.DurationMonths,
                DisplayOrder = category.DisplayOrder,
                Published = category.Published,
                CreatedOn = category.CreatedOnUtc,
                UpdatedOn = category.UpdatedOnUtc
            };

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/EditCategory.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> EditCategory(WarrantyCategoryModel model, bool continueEditing)
        {
            var category = await _warrantyService.GetWarrantyCategoryByIdAsync(model.Id);
            if (category == null)
                return RedirectToAction("Categories");

            if (ModelState.IsValid)
            {
                category.Name = model.Name;
                category.Description = model.Description;
                category.DurationMonths = model.DurationMonths;
                category.DisplayOrder = model.DisplayOrder;
                category.Published = model.Published;

                await _warrantyService.UpdateWarrantyCategoryAsync(category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyCategory.Updated"));

                if (continueEditing)
                    return RedirectToAction("EditCategory", new { id = category.Id });

                return RedirectToAction("Categories");
            }

            model.CreatedOn = category.CreatedOnUtc;
            model.UpdatedOn = category.UpdatedOnUtc;

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/EditCategory.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _warrantyService.GetWarrantyCategoryByIdAsync(id);
            if (category == null)
                return RedirectToAction("Categories");

            // Check if there are any products using this warranty category
            var mappings = await _warrantyService.GetProductWarrantyMappingsByCategoryIdAsync(id);
            if (mappings.Any())
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyCategory.CannotDelete"));
                return RedirectToAction("EditCategory", new { id });
            }

            await _warrantyService.DeleteWarrantyCategoryAsync(category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyCategory.Deleted"));

            return RedirectToAction("Categories");
        }

        #endregion

        #region Product Warranty Mappings

        public async Task<IActionResult> ProductWarrantyMappings()
        {
            var model = new WarrantyMappingModel
            {
                AvailableProducts = await PrepareProductsAsync(),
                AvailableWarrantyCategories = await PrepareWarrantyCategoriesAsync(),
                IsActive = true,
                DisplayOrder = 1
            };

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/ProductWarrantyMappings.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> WarrantyMappingList(int productId = 0)
        {
            IList<ProductWarrantyMappingRecord> mappings;
            
            // Get all product warranty mappings or filter by product ID if specified
            if (productId > 0)
            {
                // If a specific product ID is provided, get mappings for that product
                mappings = await _warrantyService.GetProductWarrantyMappingsByProductIdAsync(productId, true);
            }
            else
            {
                // If no product ID is provided, get all mappings
                // Option 1: Add a method to get all mappings directly from repository
                mappings = await _warrantyService.GetAllProductWarrantyMappingsAsync(true);
                
                // Option 2: If you haven't added GetAllProductWarrantyMappingsAsync to your service,
                // you can use this approach instead
                /*
                var allProducts = await _productService.SearchProductsAsync(pageSize: int.MaxValue, showHidden: true);
                var allMappingTasks = allProducts.Select(p => 
                    _warrantyService.GetProductWarrantyMappingsByProductIdAsync(p.Id, true));
                var allMappingResults = await Task.WhenAll(allMappingTasks);
                mappings = allMappingResults.SelectMany(x => x).ToList();
                */
            }

            var model = new List<WarrantyMappingModel>();
            
            foreach (var mapping in mappings)
            {
                var product = await _productService.GetProductByIdAsync(mapping.ProductId);
                var category = await _warrantyService.GetWarrantyCategoryByIdAsync(mapping.WarrantyCategoryId);
                
                if (product == null || category == null)
                    continue;

                model.Add(new WarrantyMappingModel
                {
                    Id = mapping.Id,
                    ProductId = mapping.ProductId,
                    ProductName = product.Name,
                    WarrantyCategoryId = mapping.WarrantyCategoryId,
                    WarrantyCategoryName = category.Name,
                    DisplayOrder = mapping.DisplayOrder,
                    IsActive = mapping.IsActive,
                    Notes = mapping.Notes
                });
            }

            return Json(new { Data = model });
        }

        [HttpPost]
        public async Task<IActionResult> AddProductWarrantyMapping(WarrantyMappingModel model)
        {
            try
            {
                // Debug logging
                System.Diagnostics.Debug.WriteLine($"Received mapping request: Product={model.ProductId}, Warranty={model.WarrantyCategoryId}");

                if (!ModelState.IsValid)
                {
                    // Log validation errors
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    System.Diagnostics.Debug.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                    return Json(new { Result = false, Errors = errors });
                }

                var mapping = new ProductWarrantyMappingRecord
                {
                    ProductId = model.ProductId,
                    WarrantyCategoryId = model.WarrantyCategoryId,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    Notes = model.Notes
                };

                await _warrantyService.InsertProductWarrantyMappingAsync(mapping);
                System.Diagnostics.Debug.WriteLine($"Successfully added mapping with ID: {mapping.Id}");

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyMapping.Added"));

                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error adding warranty mapping: {ex.Message}");
                return Json(new { Result = false, Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductWarrantyMapping(WarrantyMappingModel model)
        {
            var mapping = await _warrantyService.GetProductWarrantyMappingByIdAsync(model.Id);
            if (mapping == null)
                return Json(new { Result = false, Error = "Mapping not found" });

            if (ModelState.IsValid)
            {
                mapping.ProductId = model.ProductId;
                mapping.WarrantyCategoryId = model.WarrantyCategoryId;
                mapping.DisplayOrder = model.DisplayOrder;
                mapping.IsActive = model.IsActive;
                mapping.Notes = model.Notes;

                await _warrantyService.UpdateProductWarrantyMappingAsync(mapping);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyMapping.Updated"));

                return Json(new { Result = true });
            }

            return Json(new { Result = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductWarrantyMapping(int id)
        {
            var mapping = await _warrantyService.GetProductWarrantyMappingByIdAsync(id);
            if (mapping == null)
                return Json(new { Result = false, Error = "Mapping not found" });

            await _warrantyService.DeleteProductWarrantyMappingAsync(mapping);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyMapping.Deleted"));

            return Json(new { Result = true });
        }

        #endregion
    }
}