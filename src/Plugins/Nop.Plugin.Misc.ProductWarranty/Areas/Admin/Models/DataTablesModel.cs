using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ProductWarranty.Areas.Admin.Models
{
    public record DataTablesModel
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IList<object> Data { get; set; } = new List<object>();
    }
}

