// Migration for initial database setup
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.ProductWarranty.Domain;

namespace Nop.Plugin.Misc.ProductWarranty.Data.Migrations
{
    [NopMigration("2025/04/29 09:00:00", "Nop.Plugin.Misc.ProductWarranty initial schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            // Create warranty category table
            Create.Table(nameof(WarrantyCategoryRecord))
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(WarrantyCategoryRecord.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.Description)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(WarrantyCategoryRecord.DurationMonths)).AsInt32().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.DisplayOrder)).AsInt32().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.Published)).AsBoolean().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(WarrantyCategoryRecord.UpdatedOnUtc)).AsDateTime2().NotNullable();

            // Create warranty mapping table
            Create.Table(nameof(ProductWarrantyMappingRecord))
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(ProductWarrantyMappingRecord.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.WarrantyCategoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.DisplayOrder)).AsInt32().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.IsActive)).AsBoolean().NotNullable()
                .WithColumn(nameof(ProductWarrantyMappingRecord.Notes)).AsString(int.MaxValue).Nullable();

            // Create foreign key constraint
            Create.ForeignKey()
                .FromTable(nameof(ProductWarrantyMappingRecord))
                .ForeignColumn(nameof(ProductWarrantyMappingRecord.WarrantyCategoryId))
                .ToTable(nameof(WarrantyCategoryRecord))
                .PrimaryColumn("Id");
        }
    }
}