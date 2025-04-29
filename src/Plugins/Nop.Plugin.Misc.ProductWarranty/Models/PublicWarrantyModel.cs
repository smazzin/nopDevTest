// PublicWarrantyModel.cs
using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.ProductWarranty.Models
{
    /// <summary>
    /// Represents warranty information for display
    /// </summary>
    public record PublicWarrantyModel : BaseNopModel
    {
        public PublicWarrantyModel()
        {
            WarrantyItems = new List<WarrantyItemModel>();
        }

        /// <summary>
        /// Gets or sets a list of warranty items
        /// </summary>
        public IList<WarrantyItemModel> WarrantyItems { get; set; }

        /// <summary>
        /// Represents a single warranty item
        /// </summary>
        public record WarrantyItemModel : BaseNopModel
        {
            /// <summary>
            /// Gets or sets the warranty category ID
            /// </summary>
            public int CategoryId { get; set; }

            /// <summary>
            /// Gets or sets the warranty category name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the warranty description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the warranty duration in months
            /// </summary>
            public int DurationMonths { get; set; }

            /// <summary>
            /// Gets or sets any product-specific warranty notes
            /// </summary>
            public string Notes { get; set; }
        }
    }
}