using System;
using Nop.Core;

namespace Nop.Plugin.Misc.ProductWarranty.Domain
{
    /// <summary>
    /// Represents a product warranty mapping record
    /// </summary>
    public class ProductWarrantyMappingRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Gets or sets the warranty category identifier
        /// </summary>
        public int WarrantyCategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Gets or sets any additional warranty notes specific to this product
        /// </summary>
        public string Notes { get; set; }
    }
}