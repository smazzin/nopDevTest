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
            Create.TableFor<WarrantyCategoryRecord>();
            Create.TableFor<ProductWarrantyMappingRecord>();
        }
    }
}