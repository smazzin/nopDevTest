using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.ProductWarranty.Domain;

namespace Nop.Plugin.Misc.ProductWarranty.Data
{
    /// <summary>
    /// Represents a product warranty mapping record mapping configuration
    /// </summary>
    public partial class ProductWarrantyMappingRecordMap : NopEntityBuilder<ProductWarrantyMappingRecord>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductWarrantyMappingRecord.ProductId)).AsInt32().NotNullable().ForeignKey<Product>()
                .WithColumn(nameof(ProductWarrantyMappingRecord.WarrantyCategoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.DisplayOrder)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.IsActive)).AsBoolean().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.Notes)).AsString(int.MaxValue).Nullable();
        }
    }
}