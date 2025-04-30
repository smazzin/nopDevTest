using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    // List model for grid results
    public partial record WarrantyCategoryListModel : BaseNopEntityModel
    {
        public WarrantyCategoryListModel()
        {
            WarrantyCategories = new List<WarrantyCategoryModel>();
        }

        public List<WarrantyCategoryModel> WarrantyCategories { get; }
    }
}
