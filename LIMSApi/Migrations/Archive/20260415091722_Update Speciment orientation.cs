using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpecimentorientation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE Name = 'SpecimenOrientationID'
                    AND Object_ID = Object_ID(N'SampleDetails')
                )
                BEGIN
                    ALTER TABLE SampleDetails ADD SpecimenOrientationID bigint NULL;

                    CREATE INDEX IX_SampleDetails_SpecimenOrientationID
                    ON SampleDetails (SpecimenOrientationID);

                    ALTER TABLE SampleDetails
                    ADD CONSTRAINT FK_SampleDetails_SpecimenOrientationMasters_SpecimenOrientationID
                    FOREIGN KEY (SpecimenOrientationID) REFERENCES SpecimenOrientationMasters(ID);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE Name = 'SpecimenOrientationID'
                    AND Object_ID = Object_ID(N'SampleDetails')
                )
                BEGIN
                    ALTER TABLE SampleDetails
                    DROP CONSTRAINT FK_SampleDetails_SpecimenOrientationMasters_SpecimenOrientationID;

                    DROP INDEX IX_SampleDetails_SpecimenOrientationID ON SampleDetails;

                    ALTER TABLE SampleDetails DROP COLUMN SpecimenOrientationID;
                END
            ");
        }
    }
}
