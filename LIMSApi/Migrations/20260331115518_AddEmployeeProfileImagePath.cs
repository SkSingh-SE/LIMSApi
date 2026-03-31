using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeProfileImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'EmployeeMasters') AND name = N'ProfileImagePath')
                    ALTER TABLE EmployeeMasters ADD ProfileImagePath NVARCHAR(MAX) NULL;

                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'EmployeeMasters') AND name = N'EmployeeStatus')
                    ALTER TABLE EmployeeMasters ADD EmployeeStatus NVARCHAR(MAX) NULL;

                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'EmployeeMasters') AND name = N'DigitalSignature')
                    ALTER TABLE EmployeeMasters ADD DigitalSignature NVARCHAR(MAX) NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
