using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    public partial record WarrantyCategorySearchModel : BaseSearchModel
    {
        public WarrantyCategorySearchModel()
        {
        }

        [NopResourceDisplayName("Plugins.Misc.ProductWarranty.WarrantyCategory.Fields.Name")]
        public string Name { get; set; }
    }
}