// WarrantyMappingModel.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    /// <summary>
    /// Represents a product warranty mapping model
    /// </summary>
    public record WarrantyMappingModel : BaseNopEntityModel
    {
        public WarrantyMappingModel()
        {
            AvailableProducts = new List<SelectListItem>();
            AvailableWarrantyCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.Product")]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public IList<SelectListItem> AvailableProducts { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.WarrantyCategory")]
        public int WarrantyCategoryId { get; set; }
        public string WarrantyCategoryName { get; set; }
        public IList<SelectListItem> AvailableWarrantyCategories { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.Notes")]
        public string Notes { get; set; }
    }
}