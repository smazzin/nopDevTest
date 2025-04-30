using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    /// <summary>
    /// Represents a warranty category search model
    /// </summary>
    public partial record WarrantyCategorySearchModel : BaseSearchModel
    {
        #region Properties
        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Name")]
        public string Name { get; set; }
        #endregion
    }
}