using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedTimeDuration",
                table: "TestMethodMasters");

            migrationBuilder.DropColumn(
                name: "InvoiceCase",
                table: "TestMethodMasters");

            migrationBuilder.DropColumn(
                name: "SampleSize",
                table: "TestMethodMasters");

            migrationBuilder.DropColumn(
                name: "TestCharge",
                table: "TestMethodMasters");

            migrationBuilder.DropColumn(
                name: "TestMethodSubGroup",
                table: "TestMethodMasters");

            migrationBuilder.DropColumn(
                name: "ApprovalBy",
                table: "SupplierMasters");

            migrationBuilder.RenameColumn(
                name: "SupplierConfidentialityAgreement",
                table: "SupplierMasters",
                newName: "AgreementFilePath");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UOMMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SupplierMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConversaionFactor",
                table: "ParameterUnitMasters",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ParameterUnitID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyType",
                table: "CompanyMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "CompanyMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyCode",
                table: "CompanyMasters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CalibrationAgencyMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPerson1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPerson2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPerson3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactNo1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactNo2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactNo3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgreementFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForBlacklisting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BlacklistedBy = table.Column<long>(type: "bigint", nullable: false),
                    EvaluatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalibrationAgencyMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DisciplineMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplineMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GroupMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OEMMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPerson1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPerson2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPerson3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactNo1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactNo2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactNo3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgreementFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForBlacklisting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BlacklistedBy = table.Column<long>(type: "bigint", nullable: false),
                    EvaluatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OEMMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubGroupMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubGroupMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestMethodSubGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InvoiceCase = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestCharge = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    FixedTimeDuration = table.Column<int>(type: "int", nullable: false),
                    SampleSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSubGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodSubGroups_TestMethodMasters_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "TestMethodMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodMasters_LabDepartmentID",
                table: "TestMethodMasters",
                column: "LabDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductConditionID2",
                table: "SpecificationLines",
                column: "ProductConditionID2");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSubGroups_TestMethodID",
                table: "TestMethodSubGroups",
                column: "TestMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID2",
                table: "SpecificationLines",
                column: "ProductConditionID2",
                principalTable: "ProductConditionMasters",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestMethodMasters_DepartmentMasters_LabDepartmentID",
                table: "TestMethodMasters",
                column: "LabDepartmentID",
                principalTable: "DepartmentMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID2",
                table: "SpecificationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_TestMethodMasters_DepartmentMasters_LabDepartmentID",
                table: "TestMethodMasters");

            migrationBuilder.DropTable(
                name: "CalibrationAgencyMasters");

            migrationBuilder.DropTable(
                name: "DisciplineMasters");

            migrationBuilder.DropTable(
                name: "GroupMasters");

            migrationBuilder.DropTable(
                name: "OEMMasters");

            migrationBuilder.DropTable(
                name: "SubGroupMasters");

            migrationBuilder.DropTable(
                name: "TestMethodSubGroups");

            migrationBuilder.DropIndex(
                name: "IX_TestMethodMasters_LabDepartmentID",
                table: "TestMethodMasters");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationLines_ProductConditionID2",
                table: "SpecificationLines");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "SupplierMasters");

            migrationBuilder.DropColumn(
                name: "ConversaionFactor",
                table: "ParameterUnitMasters");

            migrationBuilder.DropColumn(
                name: "ParameterUnitID",
                table: "ParameterMasters");

            migrationBuilder.RenameColumn(
                name: "AgreementFilePath",
                table: "SupplierMasters",
                newName: "SupplierConfidentialityAgreement");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UOMMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "FixedTimeDuration",
                table: "TestMethodMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCase",
                table: "TestMethodMasters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleSize",
                table: "TestMethodMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TestCharge",
                table: "TestMethodMasters",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestMethodSubGroup",
                table: "TestMethodMasters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApprovalBy",
                table: "SupplierMasters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyType",
                table: "CompanyMasters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "CompanyMasters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyCode",
                table: "CompanyMasters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
