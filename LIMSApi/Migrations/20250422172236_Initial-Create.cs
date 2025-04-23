using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IFSCCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankMasters", x => x.ID);
                });

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
                name: "ClassificationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassificationMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CompanyCategoryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
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
                    table.PrimaryKey("PK_CompanyCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CountryMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CourierMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DesignationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignationMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DimensionalFactorMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionalFactorMasters", x => x.ID);
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
                name: "DispatchModeMasters",
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
                    table.PrimaryKey("PK_DispatchModeMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypeMasters", x => x.ID);
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
                name: "HeatTreatmentMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatTreatmentMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IndustryMasters",
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
                    table.PrimaryKey("PK_IndustryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasters",
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
                    table.PrimaryKey("PK_ItemMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MakerMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakerMasters", x => x.ID);
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
                name: "OrganisationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearSeparator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ParameterUnitMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConversaionFactor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterUnitMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PermissionMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<long>(type: "bigint", nullable: true),
                    MenuID = table.Column<long>(type: "bigint", nullable: true),
                    Viewp = table.Column<bool>(type: "bit", nullable: true),
                    Addp = table.Column<bool>(type: "bit", nullable: true),
                    Editp = table.Column<bool>(type: "bit", nullable: true),
                    Deletep = table.Column<bool>(type: "bit", nullable: true),
                    ExportP = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductConditionMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConditionMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RemarkMasters",
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
                    table.PrimaryKey("PK_RemarkMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RoleMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dashboard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SiteActivities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipaddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteActivities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SiteErrors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ipaddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteErrors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenOrientationMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenTypeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NormalCharge = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    HardCharge = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenTypeMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StandardOrganizationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardOrganizationMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubContractorMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmailID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    GSTNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubContractorMasters", x => x.ID);
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
                name: "SupplierMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    PresentStatus = table.Column<string>(type: "varchar(50)", nullable: false),
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
                    table.PrimaryKey("PK_SupplierMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TaxMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Sample = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TestCaption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InvoiceCaption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LabDepartmentID = table.Column<long>(type: "bigint", nullable: false),
                    TestDuration = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestTypeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypeMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TPIMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPIMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UniversalCodeTypeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalCodeTypeMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UOMMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UOMMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UploadFiles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadFiles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeID = table.Column<long>(type: "bigint", nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleID = table.Column<long>(type: "bigint", nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemotLogin = table.Column<bool>(type: "bit", nullable: true),
                    DeviceUser = table.Column<bool>(type: "bit", nullable: true),
                    SamplePrepare = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VendorMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TellyLedgerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GSTNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PANNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPersonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StateMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryID = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gstcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StateMasters_CountryMasters_CountryID",
                        column: x => x.CountryID,
                        principalTable: "CountryMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TestMethodMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LabDepartmentID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodMasters_DepartmentMasters_LabDepartmentID",
                        column: x => x.LabDepartmentID,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CountryID = table.Column<long>(type: "bigint", nullable: false),
                    StateID = table.Column<long>(type: "bigint", nullable: false),
                    CityID = table.Column<long>(type: "bigint", nullable: false),
                    AreaID = table.Column<long>(type: "bigint", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CurrencyID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    IndustryID = table.Column<long>(type: "bigint", nullable: false),
                    GSTNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PANNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GSTNA = table.Column<bool>(type: "bit", nullable: false),
                    TallyLedgerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SampleReturn = table.Column<bool>(type: "bit", nullable: false),
                    BillingEvery = table.Column<bool>(type: "bit", nullable: false),
                    BillingEveryDays = table.Column<int>(type: "int", nullable: true),
                    SpecialAccountingCase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WeeklyBillingCustomer = table.Column<bool>(type: "bit", nullable: false),
                    MonthlyBillingCustomer = table.Column<bool>(type: "bit", nullable: false),
                    DirectTaxInvoiceNoPerforma = table.Column<bool>(type: "bit", nullable: false),
                    PerformaInvoiceRequiredBeforeTesting = table.Column<bool>(type: "bit", nullable: false),
                    ConstantDiscount = table.Column<bool>(type: "bit", nullable: false),
                    ConstantDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditLimitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditLimitTime = table.Column<int>(type: "int", nullable: true),
                    CompanyVerified = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DTestoLoginId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTestoPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTestoActive = table.Column<bool>(type: "bit", nullable: false),
                    BlockDTestoUser = table.Column<bool>(type: "bit", nullable: false),
                    BlockReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Customers_IndustryMasters_IndustryID",
                        column: x => x.IndustryID,
                        principalTable: "IndustryMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardOrganizationID = table.Column<long>(type: "bigint", nullable: true),
                    Standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Part = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUNS = table.Column<bool>(type: "bit", nullable: true),
                    UNSSteelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AliasName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationHeaders_StandardOrganizationMasters_StandardOrganizationID",
                        column: x => x.StandardOrganizationID,
                        principalTable: "StandardOrganizationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdentificationNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TestTypeID = table.Column<long>(type: "bigint", nullable: true),
                    MakerID = table.Column<long>(type: "bigint", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentTypeID = table.Column<long>(type: "bigint", nullable: true),
                    Capacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentMasters_EquipmentTypeMasters_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalTable: "EquipmentTypeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentMasters_MakerMasters_MakerID",
                        column: x => x.MakerID,
                        principalTable: "MakerMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentMasters_TestTypeMasters_TestTypeID",
                        column: x => x.TestTypeID,
                        principalTable: "TestTypeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ParameterMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AliasName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOMID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterMasters_UOMMasters_UOMID",
                        column: x => x.UOMID,
                        principalTable: "UOMMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    ResidentialAddressLine1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ResidentialAddressLine2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ResidentialPinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResidentialAreaID = table.Column<long>(type: "bigint", nullable: false),
                    PermanentAddressLine1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PermanentAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentPinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentAreaID = table.Column<long>(type: "bigint", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    EmergencyMobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DesignationID = table.Column<long>(type: "bigint", nullable: true),
                    DateOfJoin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RelevantExperienceYears = table.Column<int>(type: "int", nullable: true),
                    PANNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountHolderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IFSCCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DepartmentID = table.Column<long>(type: "bigint", nullable: true),
                    ReportingManagerID = table.Column<long>(type: "bigint", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    IsTeamHead = table.Column<bool>(type: "bit", nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportingTo = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmployeeMasters_DepartmentMasters_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EmployeeMasters_DesignationMasters_DesignationID",
                        column: x => x.DesignationID,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EmployeeMasters_EmployeeMasters_ReportingTo",
                        column: x => x.ReportingTo,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EmployeeMasters_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CityMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateID = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CityMasters_StateMasters_StateID",
                        column: x => x.StateID,
                        principalTable: "StateMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabScopeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeMasters_TestMethodMasters_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "TestMethodMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestGroupMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestGroupID = table.Column<long>(type: "bigint", nullable: false),
                    TestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroupMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestGroupMappings_TestGroups_TestGroupID",
                        column: x => x.TestGroupID,
                        principalTable: "TestGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestGroupMappings_TestMasters_TestID",
                        column: x => x.TestID,
                        principalTable: "TestMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestGroupMappings_TestMethodMasters_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "TestMethodMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ContactPersons",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Salutation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentID = table.Column<long>(type: "bigint", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsWhatsappNo = table.Column<bool>(type: "bit", nullable: false),
                    TelephoneNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SendBill = table.Column<bool>(type: "bit", nullable: false),
                    SendReport = table.Column<bool>(type: "bit", nullable: false),
                    BillReportDeliveryAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPersons", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactPersons_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactPersons_DepartmentMasters_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCompanyCategories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCategoryID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCompanyCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerCompanyCategories_CompanyCategoryMasters_CompanyCategoryID",
                        column: x => x.CompanyCategoryID,
                        principalTable: "CompanyCategoryMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerCompanyCategories_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerDispatchModes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    DispatchModeID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerDispatchModeID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDispatchModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerDispatchModes_CustomerDispatchModes_CustomerDispatchModeID",
                        column: x => x.CustomerDispatchModeID,
                        principalTable: "CustomerDispatchModes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CustomerDispatchModes_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerDispatchModes_DispatchModeMasters_DispatchModeID",
                        column: x => x.DispatchModeID,
                        principalTable: "DispatchModeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestMethodStandards",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Caption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StandardOrganisationID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnderNABL = table.Column<bool>(type: "bit", nullable: false),
                    Group = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubGroup = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TestCategory = table.Column<string>(type: "varchar(20)", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterUnits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodStandards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodStandards_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestMethodStandards_StandardOrganizationMasters_StandardOrganisationID",
                        column: x => x.StandardOrganisationID,
                        principalTable: "StandardOrganizationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationLines",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManualSelection = table.Column<bool>(type: "bit", nullable: true),
                    ParameterID = table.Column<long>(type: "bigint", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOMID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: true),
                    DimensionalFactorID = table.Column<long>(type: "bigint", nullable: true),
                    LowerLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerLimitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpperLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperLimitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID1 = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID2 = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationLines_DimensionalFactorMasters_DimensionalFactorID",
                        column: x => x.DimensionalFactorID,
                        principalTable: "DimensionalFactorMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID1",
                        column: x => x.ProductConditionID1,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ProductConditionMasters_ProductConditionID2",
                        column: x => x.ProductConditionID2,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_UOMMasters_UOMID",
                        column: x => x.UOMID,
                        principalTable: "UOMMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocuments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<long>(type: "bigint", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdditional = table.Column<bool>(type: "bit", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocuments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmployeeDocuments_EmployeeMasters_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDocuments_UploadFiles_UploadReferenceID",
                        column: x => x.UploadReferenceID,
                        principalTable: "UploadFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeQualifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<long>(type: "bigint", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SchoolOrUniversity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PassingYear = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeQualifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EmployeeQualifications_EmployeeMasters_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityID = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AreaMasters_CityMasters_CityID",
                        column: x => x.CityID,
                        principalTable: "CityMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryID = table.Column<long>(type: "bigint", nullable: true),
                    StateID = table.Column<long>(type: "bigint", nullable: true),
                    CityID = table.Column<long>(type: "bigint", nullable: true),
                    AreaID = table.Column<long>(type: "bigint", nullable: true),
                    CurrencyID = table.Column<long>(type: "bigint", nullable: true),
                    Vatno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsiblePerson = table.Column<long>(type: "bigint", nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Gstno = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CompanyMasters_AreaMasters_AreaID",
                        column: x => x.AreaID,
                        principalTable: "AreaMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CompanyMasters_CityMasters_CityID",
                        column: x => x.CityID,
                        principalTable: "CityMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CompanyMasters_CountryMasters_CountryID",
                        column: x => x.CountryID,
                        principalTable: "CountryMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CompanyMasters_CurrencyMasters_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "CurrencyMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CompanyMasters_StateMasters_StateID",
                        column: x => x.StateID,
                        principalTable: "StateMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaMasters_CityID",
                table: "AreaMasters",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_CityMasters_StateID",
                table: "CityMasters",
                column: "StateID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMasters_AreaID",
                table: "CompanyMasters",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMasters_CityID",
                table: "CompanyMasters",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMasters_CountryID",
                table: "CompanyMasters",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMasters_CurrencyID",
                table: "CompanyMasters",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMasters_StateID",
                table: "CompanyMasters",
                column: "StateID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_CustomerID",
                table: "ContactPersons",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_DepartmentID",
                table: "ContactPersons",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCompanyCategories_CompanyCategoryID",
                table: "CustomerCompanyCategories",
                column: "CompanyCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCompanyCategories_CustomerID",
                table: "CustomerCompanyCategories",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDispatchModes_CustomerDispatchModeID",
                table: "CustomerDispatchModes",
                column: "CustomerDispatchModeID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDispatchModes_CustomerID",
                table: "CustomerDispatchModes",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDispatchModes_DispatchModeID",
                table: "CustomerDispatchModes",
                column: "DispatchModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IndustryID",
                table: "Customers",
                column: "IndustryID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeID",
                table: "EmployeeDocuments",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_UploadReferenceID",
                table: "EmployeeDocuments",
                column: "UploadReferenceID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_DepartmentID",
                table: "EmployeeMasters",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_DesignationID",
                table: "EmployeeMasters",
                column: "DesignationID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_ReportingTo",
                table: "EmployeeMasters",
                column: "ReportingTo");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_UserID",
                table: "EmployeeMasters",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeQualifications_EmployeeID",
                table: "EmployeeQualifications",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_EquipmentTypeID",
                table: "EquipmentMasters",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_MakerID",
                table: "EquipmentMasters",
                column: "MakerID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_TestTypeID",
                table: "EquipmentMasters",
                column: "TestTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeMasters_TestMethodID",
                table: "LabScopeMasters",
                column: "TestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_UOMID",
                table: "ParameterMasters",
                column: "UOMID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaders_StandardOrganizationID",
                table: "SpecificationHeaders",
                column: "StandardOrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_DimensionalFactorID",
                table: "SpecificationLines",
                column: "DimensionalFactorID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_HeatTreatmentID",
                table: "SpecificationLines",
                column: "HeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ParameterID",
                table: "SpecificationLines",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductConditionID1",
                table: "SpecificationLines",
                column: "ProductConditionID1");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductConditionID2",
                table: "SpecificationLines",
                column: "ProductConditionID2");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_SpecificationHeaderID",
                table: "SpecificationLines",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_SpecimenOrientationID",
                table: "SpecificationLines",
                column: "SpecimenOrientationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_UOMID",
                table: "SpecificationLines",
                column: "UOMID");

            migrationBuilder.CreateIndex(
                name: "IX_StateMasters_CountryID",
                table: "StateMasters",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupMappings_TestGroupID",
                table: "TestGroupMappings",
                column: "TestGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupMappings_TestID",
                table: "TestGroupMappings",
                column: "TestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupMappings_TestMethodID",
                table: "TestGroupMappings",
                column: "TestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodMasters_LabDepartmentID",
                table: "TestMethodMasters",
                column: "LabDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodStandards_EquipmentID",
                table: "TestMethodStandards",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodStandards_StandardOrganisationID",
                table: "TestMethodStandards",
                column: "StandardOrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSubGroups_TestMethodID",
                table: "TestMethodSubGroups",
                column: "TestMethodID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankMasters");

            migrationBuilder.DropTable(
                name: "CalibrationAgencyMasters");

            migrationBuilder.DropTable(
                name: "ClassificationMasters");

            migrationBuilder.DropTable(
                name: "CompanyMasters");

            migrationBuilder.DropTable(
                name: "ContactPersons");

            migrationBuilder.DropTable(
                name: "CourierMasters");

            migrationBuilder.DropTable(
                name: "CustomerCompanyCategories");

            migrationBuilder.DropTable(
                name: "CustomerDispatchModes");

            migrationBuilder.DropTable(
                name: "DisciplineMasters");

            migrationBuilder.DropTable(
                name: "EmployeeDocuments");

            migrationBuilder.DropTable(
                name: "EmployeeQualifications");

            migrationBuilder.DropTable(
                name: "GroupMasters");

            migrationBuilder.DropTable(
                name: "ItemMasters");

            migrationBuilder.DropTable(
                name: "LabScopeMasters");

            migrationBuilder.DropTable(
                name: "OEMMasters");

            migrationBuilder.DropTable(
                name: "OrganisationMasters");

            migrationBuilder.DropTable(
                name: "PermissionMasters");

            migrationBuilder.DropTable(
                name: "RemarkMasters");

            migrationBuilder.DropTable(
                name: "RoleMasters");

            migrationBuilder.DropTable(
                name: "SiteActivities");

            migrationBuilder.DropTable(
                name: "SiteErrors");

            migrationBuilder.DropTable(
                name: "SpecificationLines");

            migrationBuilder.DropTable(
                name: "SpecimenTypeMasters");

            migrationBuilder.DropTable(
                name: "SubContractorMasters");

            migrationBuilder.DropTable(
                name: "SubGroupMasters");

            migrationBuilder.DropTable(
                name: "SupplierMasters");

            migrationBuilder.DropTable(
                name: "TaxMasters");

            migrationBuilder.DropTable(
                name: "TestGroupMappings");

            migrationBuilder.DropTable(
                name: "TestMethodStandards");

            migrationBuilder.DropTable(
                name: "TestMethodSubGroups");

            migrationBuilder.DropTable(
                name: "TPIMasters");

            migrationBuilder.DropTable(
                name: "UniversalCodeTypeMasters");

            migrationBuilder.DropTable(
                name: "VendorMasters");

            migrationBuilder.DropTable(
                name: "AreaMasters");

            migrationBuilder.DropTable(
                name: "CurrencyMasters");

            migrationBuilder.DropTable(
                name: "CompanyCategoryMasters");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DispatchModeMasters");

            migrationBuilder.DropTable(
                name: "UploadFiles");

            migrationBuilder.DropTable(
                name: "EmployeeMasters");

            migrationBuilder.DropTable(
                name: "DimensionalFactorMasters");

            migrationBuilder.DropTable(
                name: "HeatTreatmentMasters");

            migrationBuilder.DropTable(
                name: "ParameterMasters");

            migrationBuilder.DropTable(
                name: "ProductConditionMasters");

            migrationBuilder.DropTable(
                name: "SpecificationHeaders");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationMasters");

            migrationBuilder.DropTable(
                name: "TestGroups");

            migrationBuilder.DropTable(
                name: "TestMasters");

            migrationBuilder.DropTable(
                name: "EquipmentMasters");

            migrationBuilder.DropTable(
                name: "TestMethodMasters");

            migrationBuilder.DropTable(
                name: "CityMasters");

            migrationBuilder.DropTable(
                name: "IndustryMasters");

            migrationBuilder.DropTable(
                name: "DesignationMasters");

            migrationBuilder.DropTable(
                name: "UserMasters");

            migrationBuilder.DropTable(
                name: "ParameterUnitMasters");

            migrationBuilder.DropTable(
                name: "UOMMasters");

            migrationBuilder.DropTable(
                name: "StandardOrganizationMasters");

            migrationBuilder.DropTable(
                name: "EquipmentTypeMasters");

            migrationBuilder.DropTable(
                name: "MakerMasters");

            migrationBuilder.DropTable(
                name: "TestTypeMasters");

            migrationBuilder.DropTable(
                name: "DepartmentMasters");

            migrationBuilder.DropTable(
                name: "StateMasters");

            migrationBuilder.DropTable(
                name: "CountryMasters");
        }
    }
}
