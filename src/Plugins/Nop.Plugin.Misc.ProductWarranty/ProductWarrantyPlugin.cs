using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.ProductWarranty
{
    /// <summary>
    /// Represents product warranty plugin
    /// </summary>
    public class ProductWarrantyPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IStoreContext _storeContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public ProductWarrantyPlugin(
            ISettingService settingService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory,
            IStoreContext storeContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _storeContext = storeContext;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory
                .GetUrlHelper(_actionContextAccessor.ActionContext)
                .RouteUrl("Admin.Plugin.Misc.ProductWarranty.Configure");
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            // Settings
            var settings = new ProductWarrantySettings
            {
                Enabled = true,
                DisplayWarrantyOnProductPage = true
            };
            await _settingService.SaveSettingAsync(settings);

            // Locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                // Plugin
                ["Plugins.Misc.ProductWarranty.Plugin.Name"] = "Product Warranty Manager",
                ["Plugins.Misc.ProductWarranty.Plugin.Description"] = "This plugin allows you to manage different types of warranties for your products",

                // Settings
                ["Plugins.Misc.ProductWarranty.Fields.Enabled"] = "Enable plugin",
                ["Plugins.Misc.ProductWarranty.Fields.DisplayWarrantyOnProductPage"] = "Display warranty on product page",

                // Categories
                ["Plugins.Misc.ProductWarranty.WarrantyCategories"] = "Warranty Categories",
                ["Plugins.Misc.ProductWarranty.WarrantyCategories.Manage"] = "Manage warranty categories",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.AddNew"] = "Add new warranty category",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.BackToList"] = "Back to warranty categories",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Edit"] = "Edit warranty category",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Created"] = "The warranty category has been created successfully.",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Updated"] = "The warranty category has been updated successfully.",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Deleted"] = "The warranty category has been deleted successfully.",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.CannotDelete"] = "The warranty category cannot be deleted because it is used by one or more products.",

                // Category fields
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Name"] = "Name",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Description"] = "Description",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.DurationMonths"] = "Duration (months)",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.DisplayOrder"] = "Display order",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Published"] = "Published",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.CreatedOn"] = "Created on",
                ["Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.UpdatedOn"] = "Updated on",

                // Mappings
                ["Plugins.Misc.ProductWarranty.WarrantyMappings"] = "Product Warranty Mappings",
                ["Plugins.Misc.ProductWarranty.WarrantyMappings.Manage"] = "Manage product warranty mappings",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.AddNew"] = "Add warranty to product",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.AddButton"] = "Add warranty",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.List"] = "Existing warranties",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Added"] = "The warranty has been added to the product successfully.",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Updated"] = "The warranty has been updated successfully.",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Deleted"] = "The warranty has been removed from the product successfully.",

                // Mapping fields
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.Product"] = "Product",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.WarrantyCategory"] = "Warranty type",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.DisplayOrder"] = "Display order",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.IsActive"] = "Is active",
                ["Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.Notes"] = "Notes",

                // Public
                ["Plugins.Misc.ProductWarranty.Public.Title"] = "Warranty Information",
                ["Plugins.Misc.ProductWarranty.Public.Month"] = "month",
                ["Plugins.Misc.ProductWarranty.Public.Months"] = "months"
            });

            // IMPORTANT: This triggers the migrations to run
            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            // Settings
            await _settingService.DeleteSettingAsync<ProductWarrantySettings>();

            // Locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.ProductWarranty");

            // IMPORTANT: This triggers the migrations to clean up
            await base.UninstallAsync();
        }

        #endregion

        #region IWidgetPlugin Implementation

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        /// <summary>
        /// Gets a value indicating whether a widget is active
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="customerRolesIds">Customer role identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the value indicating whether a widget is active
        /// </returns>
        public async Task<bool> IsWidgetActiveAsync(string widgetZone, IList<int> customerRolesIds)
        {
            if (widgetZone != PublicWidgetZones.ProductDetailsEssentialBottom)
                return false;

            // Check if the plugin is enabled
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var settings = await _settingService.LoadSettingAsync<ProductWarrantySettings>(storeId);

            return settings.Enabled && settings.DisplayWarrantyOnProductPage;
        }

        /// <summary>
        /// Gets a name of a widget view component for a specified widget zone
        /// </summary>
        /// <param name="widgetZone">Widget zone where to display the widget view component</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget view component name
        /// </returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WarrantyInfo";
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsEssentialBottom });
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where the widget should be displayed</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.ProductDetailsEssentialBottom)
                return typeof(WarrantyInfoViewComponent);

            return null;
        }

        #endregion

        #region IAdminMenuPlugin Implementation

        /// <summary>
        /// Build menu item for admin panel
        /// </summary>
        /// <param name="pluginMenuType">Menu item type</param>
        /// <returns>Menu item</returns>
        //public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        //{
        //    var contentMenu = rootNode.ChildNodes
        //        .FirstOrDefault(x => x.SystemName == "Content");

        //    // Admin menu
        //    if (pluginMenuType == AdminMenuPluginType.Plugin)
        //    {
        //        var pluginNode = new SiteMapNode
        //        {
        //            Visible = true,
        //            Title = await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.Plugin.Name"),
        //            IconClass = "far fa-dot-circle",
        //            RouteValues = new RouteValueDictionary
        //            {
        //                { "area", "Admin" },
        //                { "controller", "Warranty" },
        //                { "action", "Configure" }
        //            },
        //            ChildNodes = new List<SiteMapNode>
        //            {
        //                new SiteMapNode
        //                {
        //                    Visible = true,
        //                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyCategories"),
        //                    RouteValues = new RouteValueDictionary
        //                    {
        //                        { "area", "Admin" },
        //                        { "controller", "Warranty" },
        //                        { "action", "Categories" }
        //                    }
        //                },
        //                new SiteMapNode
        //                {
        //                    Visible = true,
        //                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.ProductWarranty.WarrantyMappings"),
        //                    RouteValues = new RouteValueDictionary
        //                    {
        //                        { "area", "Admin" },
        //                        { "controller", "Warranty" },
        //                        { "action", "ProductWarrantyMappings" }
        //                    }
        //                }
        //            }
        //        };

        //        return pluginNode;
        //    }

        //    // Other menus
        //    return null;
        //}

        #endregion
    }
}