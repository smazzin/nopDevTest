// Create this file in: src/Plugins/Nop.Plugin.Misc.ProductWarranty/Areas/Admin/Models/WarrantyMappingListModel.cs
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    public partial record WarrantyMappingListModel : BasePagedListModel<WarrantyMappingModel>
    {
        public WarrantyMappingListModel()
        {
        }
    }
}