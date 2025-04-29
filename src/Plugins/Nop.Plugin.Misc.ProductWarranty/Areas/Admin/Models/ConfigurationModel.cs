// ConfigurationModel.cs
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.Fields.Enabled")]
        public bool Enabled { get; set; }
        public bool Enabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.Fields.DisplayWarrantyOnProductPage")]
        public bool DisplayWarrantyOnProductPage { get; set; }
        public bool DisplayWarrantyOnProductPage_OverrideForStore { get; set; }
    }
}