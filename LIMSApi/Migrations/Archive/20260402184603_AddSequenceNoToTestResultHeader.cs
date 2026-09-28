using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceNoToTestResultHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add SequenceNo column (idempotent for partial migration retry)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TestResultHeaders') AND name = N'SequenceNo')
                ALTER TABLE TestResultHeaders ADD SequenceNo INT NOT NULL CONSTRAINT DF_TestResultHeaders_SequenceNo DEFAULT 1;
            ", suppressTransaction: true);

            // Step 2: Fix existing duplicate SequenceNo values using subquery (CTE doesn't work here)
            migrationBuilder.Sql(@"
                UPDATE t SET t.SequenceNo = sub.NewSeq
                FROM TestResultHeaders t
                INNER JOIN (
                    SELECT ID,
                           ROW_NUMBER() OVER (PARTITION BY SampleID, LaboratoryTestID, TestPlanID ORDER BY ID) AS NewSeq
                    FROM TestResultHeaders
                ) sub ON t.ID = sub.ID
                WHERE sub.NewSeq > 1;
            ", suppressTransaction: true);

            // Step 3: Create unique index (idempotent)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE object_id = OBJECT_ID(N'TestResultHeaders')
                    AND name = N'IX_TestResultHeaders_Sample_LabTest_Plan_Seq'
                )
                CREATE UNIQUE INDEX IX_TestResultHeaders_Sample_LabTest_Plan_Seq
                ON TestResultHeaders (SampleID, LaboratoryTestID, TestPlanID, SequenceNo);
            ", suppressTransaction: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReportNo",
                table: "GeneralTestMethods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GSTNo",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ParameterUnit",
                table: "ChemicalTestElements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestResultHeaders_Sample_LabTest_Plan_Seq",
                table: "TestResultHeaders");

            migrationBuilder.DropColumn(
                name: "SequenceNo",
                table: "TestResultHeaders");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "SampleDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReportNo",
                table: "GeneralTestMethods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GSTNo",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParameterUnit",
                table: "ChemicalTestElements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
