using Nop.Core;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    // List model for grid results
    public partial record WarrantyCategoryListModel : BasePagedListModel<WarrantyCategoryModel>
    {
        public WarrantyCategoryListModel()
        {
        }
    }
}
