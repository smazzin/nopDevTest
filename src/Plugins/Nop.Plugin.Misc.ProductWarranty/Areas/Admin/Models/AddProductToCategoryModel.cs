using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    /// <summary>
    /// Represents a model for adding a product to a warranty category
    /// </summary>
    public record AddProductToCategoryModel : BaseNopModel
    {
        #region Ctor

        public AddProductToCategoryModel()
        {
            SelectedProductIds = new List<int>();
        }

        #endregion

        #region Properties

        public int CategoryId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}