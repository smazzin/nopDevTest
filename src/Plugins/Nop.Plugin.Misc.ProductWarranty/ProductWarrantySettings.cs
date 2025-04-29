using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.ProductWarranty
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class ProductWarrantySettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled
        /// </summary>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to display warranty information on product page
        /// </summary>
        public bool DisplayWarrantyOnProductPage { get; set; }
    }
}