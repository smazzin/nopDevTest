using Microsoft.AspNetCore.Http;
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
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
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
        private readonly IManufacturerService _manufacturerService;
        private readonly ICategoryService _categoryService;
        private readonly IProductModelFactory _productModelFactory;

        #endregion

        #region Ctor

        public WarrantyController(
            IWarrantyService warrantyService,
            ISettingService settingService,
            IStoreContext storeContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IProductService productService,
            IManufacturerService manufacturerService,
            ICategoryService categoryService,
            IProductModelFactory productModelFactory)
        {
            _warrantyService = warrantyService;
            _settingService = settingService;
            _storeContext = storeContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _productService = productService;
            _manufacturerService = manufacturerService;
            _categoryService = categoryService;
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

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> Categories()
        {
            // Initialize the search model
            var model = new WarrantyCategorySearchModel();

            // Set the grid page size - this is crucial!
            model.SetGridPageSize();

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/Categories.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> CategoryList(WarrantyCategorySearchModel searchModel)
        {
            // Get all categories
            var categories = await _warrantyService.GetAllWarrantyCategoriesAsync(true);

            // Create paged list
            var pagedList = new PagedList<WarrantyCategoryRecord>(
                categories,
                searchModel.Page - 1,
                searchModel.PageSize);

            // Prepare the model
            var model = new WarrantyCategoryListModel().PrepareToGrid<WarrantyCategoryListModel, WarrantyCategoryModel, WarrantyCategoryRecord>(
                searchModel,
                pagedList,
                () =>
                {
                    return pagedList.Select(category =>
                    {
                        var categoryModel = new WarrantyCategoryModel
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Description = category.Description ?? "",
                            DurationMonths = category.DurationMonths,
                            DisplayOrder = category.DisplayOrder,
                            Published = category.Published,
                            CreatedOn = category.CreatedOnUtc,
                            UpdatedOn = category.UpdatedOnUtc
                        };

                        return categoryModel;
                    });
                });

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
            if (category == null)
                return RedirectToAction("Categories");

            var model = new WarrantyCategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                DurationMonths = category.DurationMonths,
                DisplayOrder = category.DisplayOrder,
                Published = category.Published,
                CreatedOn = category.CreatedOnUtc,
                UpdatedOn = category.UpdatedOnUtc,
                // Add available products for mappings
                AvailableProducts = await PrepareProductsAsync()
            };

            //WarrantySearchModel = new WarrantyCategorySearchModel();
            model.WarrantySearchModel.SetGridPageSize();

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/EditCategory.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryProductMappingList(WarrantyCategorySearchModel searchModel)
        {
            // Get the category id from various possible sources
            int categoryId = 0;

            // Check if it's in the route values first
            if (Request.RouteValues.ContainsKey("categoryId") && int.TryParse(Request.RouteValues["categoryId"].ToString(), out int routeId))
            {
                categoryId = routeId;
            }
            // Then check query string
            else if (Request.Query.ContainsKey("categoryId") && int.TryParse(Request.Query["categoryId"], out int queryId))
            {
                categoryId = queryId;
            }
            // Then check if it's in the search model
            else if (searchModel != null && searchModel.Id > 0)
            {
                categoryId = searchModel.Id;
            }
            // Finally check form data
            else if (Request.HasFormContentType && Request.Form.ContainsKey("categoryId") &&
                     int.TryParse(Request.Form["categoryId"], out int formId))
            {
                categoryId = formId;
            }

            // Verify we have a valid category ID
            if (categoryId == 0)
            {
                return Json(new { Result = false, Error = "Category ID not provided" });
            }

            // Get the category
            var category = await _warrantyService.GetWarrantyCategoryByIdAsync(categoryId);
            if (category == null)
                return Json(new { Result = false, Error = "Category not found" });

            // Get mappings for this category
            var mappings = await _warrantyService.GetProductWarrantyMappingsByCategoryIdAsync(categoryId);

            // Create a list to hold the mapped data
            var mappingModels = new List<WarrantyMappingModel>();

            foreach (var mapping in mappings)
            {
                var product = await _productService.GetProductByIdAsync(mapping.ProductId);
                if (product == null)
                    continue;

                mappingModels.Add(new WarrantyMappingModel
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

            // Create paged list
            var pagedList = new PagedList<WarrantyMappingModel>(
                mappingModels,
                searchModel.Page - 1,
                searchModel.PageSize);

            // Prepare model for the grid
            var model = new WarrantyMappingListModel().PrepareToGrid(
                searchModel,
                pagedList,
                () => pagedList);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryProductMapping(WarrantyMappingModel model)
        {
            if (ModelState.IsValid)
            {
                var mapping = new ProductWarrantyMappingRecord
                {
                    ProductId = model.ProductId,
                    WarrantyCategoryId = model.WarrantyCategoryId,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    Notes = model.Notes
                };

                await _warrantyService.InsertProductWarrantyMappingAsync(mapping);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyMapping.Added"));

                return Json(new { Result = true });
            }

            return Json(new { Result = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> GetFilteredProductsForMapping(int categoryId, int manufacturerId = 0, int productCategoryId = 0)
        {
            // Get products based on filters
            var products = await _productService.SearchProductsAsync(
                manufacturerIds: manufacturerId > 0 ? new List<int> { manufacturerId } : null,
                categoryIds: productCategoryId > 0 ? new List<int> { productCategoryId } : null,
                showHidden: true,
                pageSize: int.MaxValue);

            // Get existing mappings for this category to mark as selected
            var existingMappings = await _warrantyService.GetProductWarrantyMappingsByCategoryIdAsync(categoryId);
            var existingProductIds = existingMappings.Select(m => m.ProductId).ToList();

            // Prepare model
            var model = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Price = p.Price,
                Published = p.Published,
                Selected = existingProductIds.Contains(p.Id)
            }).ToList();

            return Json(new { Data = model });
        }

        #region Product Popup Methods

        [HttpGet]
        public async Task<IActionResult> ProductAddPopup(int categoryId)
        {

            // Create a simple search model without relying on IProductModelFactory
            var model = new ProductSearchModel();

            // Initialize the model manually with the necessary data
            model.SetGridPageSize();

            // Prepare available categories
            model.AvailableCategories.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
                Value = "0"
            });

            var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            foreach (var category in categories)
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Text = await _categoryService.GetFormattedBreadCrumbAsync(category),
                    Value = category.Id.ToString()
                });
            }

            // Prepare available manufacturers
            model.AvailableManufacturers.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
                Value = "0"
            });

            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
            foreach (var manufacturer in manufacturers)
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Text = manufacturer.Name,
                    Value = manufacturer.Id.ToString()
                });
            }

            // Store the category ID in ViewBag to be used in the form
            ViewBag.CategoryId = categoryId;

            return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/ProductAddPopup.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductAddPopupList(ProductSearchModel searchModel)
        {

            // Search for products directly
            var products = await _productService.SearchProductsAsync(
                categoryIds: searchModel.SearchCategoryId > 0 ? new List<int> { searchModel.SearchCategoryId } : null,
                manufacturerIds: searchModel.SearchManufacturerId > 0 ? new List<int> { searchModel.SearchManufacturerId } : null,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize,
                showHidden: true);

            // Create response model
            var model = new ProductListModel
            {
                Data = products.Select(product => new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Published = product.Published
                }).ToList(),
                Draw = searchModel.Draw,
                RecordsTotal = products.TotalCount,
                RecordsFiltered = products.TotalCount
            };

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductAddPopup(int categoryId, [FromForm] IFormCollection form)
        {
            try
            {
                // Get the warranty category
                var category = await _warrantyService.GetWarrantyCategoryByIdAsync(categoryId);
                if (category == null)
                    return RedirectToAction("Categories");

                // Get the selected product IDs string from form
                var selectedIdsString = form["selectedIds"].ToString();
                Console.WriteLine($"Selected IDs string: {selectedIdsString}");

                // Convert to list of integers
                var selectedIds = selectedIdsString
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out int parsedId) ? parsedId : 0)
                    .Where(id => id > 0)
                    .ToList();

                Console.WriteLine($"Parsed product IDs: {string.Join(", ", selectedIds)}");

                if (selectedIds.Any())
                {
                    // Get existing mappings
                    var existingMappings = await _warrantyService.GetProductWarrantyMappingsByCategoryIdAsync(categoryId);
                    var existingProductIds = existingMappings.Select(m => m.ProductId).ToList();

                    // Filter out products that are already mapped
                    var newProductIds = selectedIds.Except(existingProductIds).ToList();
                    Console.WriteLine($"New product IDs to add: {string.Join(", ", newProductIds)}");

                    // Add new mappings
                    foreach (var productId in newProductIds)
                    {
                        var mapping = new ProductWarrantyMappingRecord
                        {
                            ProductId = productId,
                            WarrantyCategoryId = categoryId,
                            DisplayOrder = 1,
                            IsActive = true
                        };

                        await _warrantyService.InsertProductWarrantyMappingAsync(mapping);
                        Console.WriteLine($"Added product ID: {productId}");
                    }
                }

                // Set flag to refresh the parent window
                ViewBag.RefreshPage = true;

                return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/ProductAddPopup.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductAddPopup: {ex.Message}");
                // Return with error
                return View("~/Plugins/Misc.ProductWarranty/Areas/Admin/Views/Warranty/ProductAddPopup.cshtml");
            }
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> GetManufacturers()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
            var model = manufacturers.Select(m => new { Id = m.Id, Name = m.Name }).ToList();
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            var model = categories.Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            return Json(model);
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

            //model.SetGridPageSize();

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
                mappings = await _warrantyService.GetAllProductWarrantyMappingsAsync(true);
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

            // Return in the format expected by DataTables
            return Json(new
            {
                draw = 1,
                recordsTotal = model.Count,
                recordsFiltered = model.Count,
                data = model
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddProductWarrantyMapping(WarrantyMappingModel model)
        {
            if (ModelState.IsValid)
            {
                var mapping = new ProductWarrantyMappingRecord
                {
                    ProductId = model.ProductId,
                    WarrantyCategoryId = model.WarrantyCategoryId,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    Notes = model.Notes
                };

                await _warrantyService.InsertProductWarrantyMappingAsync(mapping);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyMapping.Added"));

                return Json(new { Result = true });
            }

            return Json(new { Result = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
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