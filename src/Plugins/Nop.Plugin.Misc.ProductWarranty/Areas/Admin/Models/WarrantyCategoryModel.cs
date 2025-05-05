// WarrantyCategoryModel.cs
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    /// <summary>
    /// Represents a warranty category model
    /// </summary>
    public record WarrantyCategoryModel : BaseNopEntityModel
    {
        public WarrantyCategoryModel()
        {
            AvailableProducts = new List<SelectListItem>();
            WarrantySearchModel = new WarrantyCategorySearchModel();
        }

        // Existing properties...
        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.DurationMonths")]
        public int DurationMonths { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        // Add these new properties
        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.Notes")]
        public string Notes { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyMapping.Fields.IsActive")]
        public bool IsActive { get; set; }

        public IList<SelectListItem> AvailableProducts { get; set; }
        public WarrantyCategorySearchModel WarrantySearchModel { get; }
    }
}