using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class SyncAllPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SpecimenOrientationID column, index, and FK already exist in SampleDetails
            // from migration 20260320092838_AddOrientationDeviationAcknowledged
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: column managed by earlier migration
        }
    }
}
