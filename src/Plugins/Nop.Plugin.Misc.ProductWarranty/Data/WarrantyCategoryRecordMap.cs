using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.ProductWarranty.Domain;

namespace Nop.Plugin.Misc.ProductWarranty.Data
{
    /// <summary>
    /// Represents a warranty category record mapping configuration
    /// </summary>
    public partial class WarrantyCategoryRecordMap : NopEntityBuilder<WarrantyCategoryRecord>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WarrantyCategoryRecord.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.Description)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(WarrantyCategoryRecord.DurationMonths)).AsInt32().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.DisplayOrder)).AsInt32().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.Published)).AsBoolean().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.UpdatedOnUtc)).AsDateTime2().NotNullable();
        }
    }
}