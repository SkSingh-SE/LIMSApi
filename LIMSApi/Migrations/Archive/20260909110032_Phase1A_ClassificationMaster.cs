using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class Phase1A_ClassificationMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisTechniqueMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AliasNames = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisTechniqueMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizedSignatories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignaturePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicableFor = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedSignatories", x => x.Id);
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
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    AgreementFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_CalibrationAgencyMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ClassificationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                name: "Configurations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CoolingMediumMasters",
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
                    table.PrimaryKey("PK_CoolingMediumMasters", x => x.ID);
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
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CountryID = table.Column<long>(type: "bigint", nullable: false),
                    StateID = table.Column<long>(type: "bigint", nullable: false),
                    CityID = table.Column<long>(type: "bigint", nullable: false),
                    AreaID = table.Column<long>(type: "bigint", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CurrencyID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    GSTNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerStateCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    PANNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GSTNA = table.Column<bool>(type: "bit", nullable: false),
                    TallyLedgerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SampleReturn = table.Column<bool>(type: "bit", nullable: false),
                    BillingEvery = table.Column<bool>(type: "bit", nullable: false),
                    BillingEveryDays = table.Column<int>(type: "int", nullable: true),
                    LastBillingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpecialAccountingCase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WeeklyBillingCustomer = table.Column<bool>(type: "bit", nullable: false),
                    MonthlyBillingCustomer = table.Column<bool>(type: "bit", nullable: false),
                    DirectTaxInvoiceNoPerforma = table.Column<bool>(type: "bit", nullable: false),
                    PerformaInvoiceRequiredBeforeTesting = table.Column<bool>(type: "bit", nullable: false),
                    ConstantDiscount = table.Column<bool>(type: "bit", nullable: false),
                    ConstantDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditLimitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditLimitTime = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DTestoLoginId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTestoPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTestoActive = table.Column<bool>(type: "bit", nullable: false),
                    BlockDTestoUser = table.Column<bool>(type: "bit", nullable: false),
                    BlockReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<long>(type: "bigint", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "DisciplineMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                name: "ExecutionConfigSnapshots",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnapshotHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionConfigSnapshots", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FinancialYears",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GstConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GstNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PanNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DefaultGstRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PIGstApplicable = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GstConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HardnessEquivalences",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HardnessScale = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IndenterSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoadKgf = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FromValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EquivalentScale = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EquivalentFromValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EquivalentToValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Standard = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardnessEquivalences", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HeatTreatmentCategoryMasters",
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
                    table.PrimaryKey("PK_HeatTreatmentCategoryMasters", x => x.ID);
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
                name: "JobExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabScopeChangeLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeID = table.Column<long>(type: "bigint", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedBy = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeChangeLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MachineDataLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    TestResultHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    RawPayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchedCount = table.Column<int>(type: "int", nullable: false),
                    UnmatchedCount = table.Column<int>(type: "int", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineSerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineDataLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MachiningChargeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodStandardID = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenRawMaterialSize = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SpecimenSize = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    DrawingFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachiningChargeMasters", x => x.ID);
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
                name: "MenuMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsExpanded = table.Column<bool>(type: "bit", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MenuMasters_MenuMasters_ParentID",
                        column: x => x.ParentID,
                        principalTable: "MenuMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MetalClassificationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ParentID = table.Column<long>(type: "bigint", nullable: true),
                    HasChemicalParams = table.Column<bool>(type: "bit", nullable: false),
                    HasMechanicalParams = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    MetalType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetalClassificationMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MetalClassificationMasters_MetalClassificationMasters_ParentID",
                        column: x => x.ParentID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablApprovedSuppliers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsApproved = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PerformanceRating = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablApprovedSuppliers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablAuditLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormDataId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedBy = table.Column<long>(type: "bigint", nullable: false),
                    PerformedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablDocumentChangeRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForChange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UrgencyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssessedImpact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessmentBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImplementationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablDocumentChangeRequests", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablDocumentReviews",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentRevision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeRequired = table.Column<bool>(type: "bit", nullable: true),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablDocumentReviews", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablFormAttachments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormDataId = table.Column<long>(type: "bigint", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFormAttachments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablFormRevisionHistory",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormDataId = table.Column<long>(type: "bigint", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisedBy = table.Column<long>(type: "bigint", nullable: false),
                    RevisedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFormRevisionHistory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMasterDocuments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentIssue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentRevision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentOwner = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ControlledCopiesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObsoleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMasterDocuments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMeasurementUncertainties",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MatrixType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UncertaintyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourcesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CombinedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ExpandedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CoverageFactor = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ConfidenceLevel = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMeasurementUncertainties", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMethodValidations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMatrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationScope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SelectivityResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinearityRange = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DetectionLimit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuantificationLimit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrecisionRSD = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    BiasPercentage = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    RobustnessResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UncertaintyResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OverallConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValidatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMethodValidations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablMethodVerifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMatrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LinearityResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrecisionResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BiasResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UncertaintyResults = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OverallConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMethodVerifications", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablNonConformingWorks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SampleCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NCDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DetectedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IdentifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SuspendedWork = table.Column<bool>(type: "bit", nullable: true),
                    AffectedResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImmediateAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RootCauseAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNonConformingWorks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablPtIlcPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanYear = table.Column<int>(type: "int", nullable: true),
                    PTType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrganizingBody = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScheduleJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalParticipations = table.Column<int>(type: "int", nullable: true),
                    SatisfactoryResults = table.Column<int>(type: "int", nullable: true),
                    UnsatisfactoryResults = table.Column<int>(type: "int", nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OverallAssessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPtIlcPlans", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablReferenceMaterials",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RMCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RMName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StorageCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CertifiedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Uncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    QuantityUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablReferenceMaterials", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablRetestings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OriginalTestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetestReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetestResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetestConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablRetestings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablRiskAssessments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessArea = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RisksJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallRiskLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssessedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablRiskAssessments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSampleInwardRegisters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleInwardId = table.Column<long>(type: "bigint", nullable: true),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestsRequested = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSampleInwardRegisters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSampleLabels",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LabelNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StorageCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LabelledBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSampleLabels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSampleMusterRegisters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MusteringDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MusteredBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumberOfPieces = table.Column<int>(type: "int", nullable: true),
                    SampleDimensions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CuttingInstructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreparedSamples = table.Column<int>(type: "int", nullable: true),
                    WasteGenerated = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisposalMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSampleMusterRegisters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSupplierConfidentialities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreementValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfidentialItems = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PenaltyClause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupplierSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSupplierConfidentialities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSupplierEvaluations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalScore = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    PercentageScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EvaluationResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextEvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSupplierEvaluations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablSupplierRegistrations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NatureOfBusiness = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductsServicesOffered = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemsSupplied = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GstNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PanNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsoCertified = table.Column<bool>(type: "bit", nullable: false),
                    IsoDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BankDetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentsSubmittedJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NablApproved = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSupplierRegistrations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NablTestReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestResultsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SamplingDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MethodReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Conclusion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Disclaimer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTestReports", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityID = table.Column<long>(type: "bigint", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: true),
                    StepID = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.ID);
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
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    AgreementFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LabCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LabAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationLogo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CIN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UlrPrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LabLocationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsMultiBranch = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParameterUnitMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
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
                name: "PriceDimensionTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsRange = table.Column<bool>(type: "bit", nullable: false),
                    ValueSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SampleField = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceDimensionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductConditionCategoryMasters",
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
                    table.PrimaryKey("PK_ProductConditionCategoryMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductFormMasters",
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
                    table.PrimaryKey("PK_ProductFormMasters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTypeMasters",
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
                    table.PrimaryKey("PK_PropertyTypeMasters", x => x.ID);
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
                name: "ReportFormats",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormatName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PageLayout = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PageSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeaderConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFormats", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RoleMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
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
                name: "SampleStatusHistories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EntityID = table.Column<long>(type: "bigint", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedBy = table.Column<long>(type: "bigint", nullable: false),
                    ChangedByName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleStatusHistories", x => x.ID);
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
                name: "SpecimenOrientationCategoryMasters",
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
                    table.PrimaryKey("PK_SpecimenOrientationCategoryMasters", x => x.ID);
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
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PresentStatus = table.Column<string>(type: "varchar(50)", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    AgreementFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: false),
                    ReasonForBlacklisting = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BlacklistedBy = table.Column<long>(type: "bigint", nullable: true),
                    BlacklistDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
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
                name: "TestConditionDimensions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConditionDimensions", x => x.ID);
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
                name: "UserPushSubscriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    P256DH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Auth = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPushSubscriptions", x => x.Id);
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
                name: "Workflows",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestMethodSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: true),
                    StandardOrganizationID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodStandard = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Part = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayTitle = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    LinkedStandard = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FormulaExpression = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultParameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodSpecifications_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
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
                name: "ContactPersons",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Salutation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsWhatsappNo = table.Column<bool>(type: "bit", nullable: false),
                    TelephoneNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SendBill = table.Column<bool>(type: "bit", nullable: false),
                    SendReport = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreaID = table.Column<long>(type: "bigint", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "CustomerChangeRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    OldValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewedBy = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkflowInstanceID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerChangeRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerChangeRequests_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
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
                name: "CustomerPurchaseOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    POAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UtilizedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Terms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPurchaseOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerPurchaseOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NablComplaints",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ComplaintDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComplaintDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComplaintCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SampleCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvestigationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerInformedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerSatisfied = table.Column<bool>(type: "bit", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablComplaints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablComplaints_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablCustomerFeedbacks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeedbackPeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeedbackPeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RatingsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallSatisfaction = table.Column<int>(type: "int", nullable: true),
                    TurnaroundRating = table.Column<int>(type: "int", nullable: true),
                    AccuracyRating = table.Column<int>(type: "int", nullable: true),
                    CommunicationRating = table.Column<int>(type: "int", nullable: true),
                    ServiceRating = table.Column<int>(type: "int", nullable: true),
                    CommentsSuggestions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suggestions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WouldRecommend = table.Column<bool>(type: "bit", nullable: true),
                    CollectedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCustomerFeedbacks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCustomerFeedbacks_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTestRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SampleQuantity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleCondition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenceStandard = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestPurpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTestRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTestRequests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentReceipts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChequeNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InvoiceIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentReceipts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_GroupMasters_DisciplineMasters_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "DisciplineMasters",
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
                    DispatchModeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDispatchModes", x => x.ID);
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
                name: "HeatTreatmentMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HeatTreatmentCategoryID = table.Column<long>(type: "bigint", nullable: true),
                    TempRangeMin = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TempRangeMax = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TempRangeDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CoolingMediumID = table.Column<long>(type: "bigint", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_HeatTreatmentMasters_CoolingMediumMasters_CoolingMediumID",
                        column: x => x.CoolingMediumID,
                        principalTable: "CoolingMediumMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_HeatTreatmentMasters_HeatTreatmentCategoryMasters_HeatTreatmentCategoryID",
                        column: x => x.HeatTreatmentCategoryID,
                        principalTable: "HeatTreatmentCategoryMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MachiningChargeVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachiningChargeMasterID = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    PriceGeneralMetal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceHardMetal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CuttingRateGeneralMetal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CuttingRateHardMetal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachiningChargeVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachiningChargeVersions_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MachiningChargeVersions_MachiningChargeMasters_MachiningChargeMasterID",
                        column: x => x.MachiningChargeMasterID,
                        principalTable: "MachiningChargeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PermissionMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuID = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PermissionMasters_MenuMasters_MenuID",
                        column: x => x.MenuID,
                        principalTable: "MenuMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MetalClassificationAnalysisTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetalClassificationAnalysisTechniques", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MetalClassificationAnalysisTechniques_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MetalClassificationAnalysisTechniques_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablNcCorrectiveActions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCId = table.Column<long>(type: "bigint", nullable: true),
                    NCRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CADate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImplementedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImplementationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EffectivenessEvaluated = table.Column<bool>(type: "bit", nullable: true),
                    EffectivenessResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: true),
                    ClosureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablNcCorrectiveActions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablNcCorrectiveActions_NablNonConformingWorks_NCId",
                        column: x => x.NCId,
                        principalTable: "NablNonConformingWorks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablCrmConsumptions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceMaterialId = table.Column<long>(type: "bigint", nullable: true),
                    RMCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RMName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantityUsed = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    QuantityUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurposeOfUse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UsedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RemainingAfterUse = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsExhausted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCrmConsumptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCrmConsumptions_NablReferenceMaterials_ReferenceMaterialId",
                        column: x => x.ReferenceMaterialId,
                        principalTable: "NablReferenceMaterials",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsHeadOffice = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Branches_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConditionMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    AllowedOperators = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AllowedValuesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ConditionMasters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParameterUnitEquivalents",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseParameterUnitID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterUnitEquivalents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterUnitEquivalents_ParameterUnitMasters_BaseParameterUnitID",
                        column: x => x.BaseParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSizeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizeMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductSizeMasters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCaseConfigurations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AliasName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Start = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    End = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceParameterIDs = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsBaseConfig = table.Column<bool>(type: "bit", nullable: false),
                    FallbackToUserInput = table.Column<bool>(type: "bit", nullable: false),
                    PriceDimensionTypeId = table.Column<long>(type: "bigint", nullable: true),
                    OverrideParameterIDs = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCaseConfigurations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceCaseConfigurations_PriceDimensionTypes_PriceDimensionTypeId",
                        column: x => x.PriceDimensionTypeId,
                        principalTable: "PriceDimensionTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReportFormatSections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFormatID = table.Column<long>(type: "bigint", nullable: false),
                    SectionType = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFormatSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportFormatSections_ReportFormats_ReportFormatID",
                        column: x => x.ReportFormatID,
                        principalTable: "ReportFormats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DesignationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    RoleID = table.Column<long>(type: "bigint", nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinExperience = table.Column<int>(type: "int", nullable: true),
                    PersonalityTraits = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    RolesAndResponsibilities = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
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
                    table.ForeignKey(
                        name: "FK_DesignationMasters_RoleMasters_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RoleMenuMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<long>(type: "bigint", nullable: false),
                    MenuID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenuMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RoleMenuMappings_MenuMasters_MenuID",
                        column: x => x.MenuID,
                        principalTable: "MenuMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenuMappings_RoleMasters_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SpecimenOrientationCategoryID = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationMasters_SpecimenOrientationCategoryMasters_SpecimenOrientationCategoryID",
                        column: x => x.SpecimenOrientationCategoryID,
                        principalTable: "SpecimenOrientationCategoryMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CuttingPriceMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecimenTypeId = table.Column<long>(type: "bigint", nullable: true),
                    CuttingType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeRangeMin = table.Column<decimal>(type: "decimal(7,2)", nullable: true),
                    SizeRangeMax = table.Column<decimal>(type: "decimal(7,2)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingPriceMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingPriceMasters_SpecimenTypeMasters_SpecimenTypeId",
                        column: x => x.SpecimenTypeId,
                        principalTable: "SpecimenTypeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SpecificationHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AliasName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardReference = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    StandardOrganizationID = table.Column<long>(type: "bigint", nullable: true),
                    Standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Part = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false),
                    SpecificationNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IdentifierConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                name: "NablIncomingMaterials",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BatchNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablIncomingMaterials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablIncomingMaterials_SupplierMasters_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablProductInspections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleSize = table.Column<int>(type: "int", nullable: true),
                    DefectsFound = table.Column<int>(type: "int", nullable: true),
                    InspectionCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InspectionResultsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablProductInspections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablProductInspections_SupplierMasters_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    AssignedToType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedToValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimensionalFactorMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    Instrument = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToleranceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DefaultTestMethodID = table.Column<long>(type: "bigint", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_DimensionalFactorMasters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DimensionalFactorMasters_TestMethodSpecifications_DefaultTestMethodID",
                        column: x => x.DefaultTestMethodID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodStandardId = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestMethodTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMatrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Principle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicableStandard = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EquipmentRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReagentsRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SamplePreparation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Procedure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalibrationRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QualityControlRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UncertaintyStatement = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DetectionLimit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTestMethods_TestMethodSpecifications_TestMethodStandardId",
                        column: x => x.TestMethodStandardId,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TestMethodSpecificationMetalClassifications",
                columns: table => new
                {
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSpecificationMetalClassifications", x => new { x.TestMethodSpecificationID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationMetalClassifications_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestMethodSpecificationVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupersededDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StandardFile = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StandardFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSpecificationVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationVersions_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerPurchaseOrderID = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BilledAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_CustomerPurchaseOrders_CustomerPurchaseOrderID",
                        column: x => x.CustomerPurchaseOrderID,
                        principalTable: "CustomerPurchaseOrders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablFeedbackAnalyses",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisPeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnalysisPeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalFeedbacks = table.Column<int>(type: "int", nullable: true),
                    AverageSatisfaction = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageTurnaround = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageAccuracy = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageCommunication = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AverageService = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AcceptanceCriteria = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MeetsAcceptanceCriteria = table.Column<bool>(type: "bit", nullable: true),
                    KeyStrengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnalysedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerFeedbackId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablFeedbackAnalyses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablFeedbackAnalyses_NablCustomerFeedbacks_CustomerFeedbackId",
                        column: x => x.CustomerFeedbackId,
                        principalTable: "NablCustomerFeedbacks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SubGroupMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GroupID = table.Column<long>(type: "bigint", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_SubGroupMasters_GroupMasters_GroupID",
                        column: x => x.GroupID,
                        principalTable: "GroupMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeatTreatmentMetalClassifications",
                columns: table => new
                {
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatTreatmentMetalClassifications", x => new { x.HeatTreatmentID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_HeatTreatmentMetalClassifications_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeatTreatmentMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductConditionMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProductConditionCategoryID = table.Column<long>(type: "bigint", nullable: true),
                    LinkedHeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
                    CalibrationRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsDestructive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_ProductConditionMasters_HeatTreatmentMasters_LinkedHeatTreatmentID",
                        column: x => x.LinkedHeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductConditionMasters_ProductConditionCategoryMasters_ProductConditionCategoryID",
                        column: x => x.ProductConditionCategoryID,
                        principalTable: "ProductConditionCategoryMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<long>(type: "bigint", nullable: false),
                    PermissionID = table.Column<long>(type: "bigint", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RolePermissions_PermissionMasters_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "PermissionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RolePermissions_RoleMasters_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMasters",
                        principalColumn: "ID");
                });

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
                    BranchID = table.Column<long>(type: "bigint", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_BankMasters_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchDisciplines",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: false),
                    IsAccredited = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchDisciplines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BranchDisciplines_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchDisciplines_DisciplineMasters_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "DisciplineMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChemical = table.Column<bool>(type: "bit", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_DepartmentMasters_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentMasters_DisciplineMasters_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "DisciplineMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NablAccreditations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAccreditations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NablAccreditations_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NablAccreditations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NumberingConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartNumber = table.Column<int>(type: "int", nullable: false),
                    CurrentNumber = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberingConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumberingConfigs_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumberingConfigs_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SampleInwards",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GstNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AdvancePayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillRequired = table.Column<bool>(type: "bit", nullable: false),
                    AdvancePIRequired = table.Column<bool>(type: "bit", nullable: false),
                    HoldTesting = table.Column<bool>(type: "bit", nullable: false),
                    HoldTestingUntilPIApproved = table.Column<bool>(type: "bit", nullable: false),
                    Urgent = table.Column<bool>(type: "bit", nullable: false),
                    ReturnSample = table.Column<bool>(type: "bit", nullable: false),
                    NotDestroyed = table.Column<bool>(type: "bit", nullable: false),
                    SampleReceiptNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StatementOfConformity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionRule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestFilePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RequestFileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    InwardStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CollectionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedBy = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PIReceived = table.Column<bool>(type: "bit", nullable: false),
                    IsInvoiceGenerated = table.Column<bool>(type: "bit", nullable: false),
                    IsReportUnlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsAmendmentAllowed = table.Column<bool>(type: "bit", nullable: false),
                    IsReportStopped = table.Column<bool>(type: "bit", nullable: false),
                    StopReportReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StopReportOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StopReportBy = table.Column<long>(type: "bigint", nullable: true),
                    TotalTestCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleInwards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleInwards_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SampleInwards_CustomerPurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "CustomerPurchaseOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SampleInwards_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleInwards_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ParameterMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParameterType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitEquivalentID = table.Column<long>(type: "bigint", nullable: true),
                    UnitConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    DecimalPrecision = table.Column<int>(type: "int", nullable: false),
                    IsCalculated = table.Column<bool>(type: "bit", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormulaDisplay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElementType = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                        name: "FK_ParameterMasters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                        column: x => x.ParameterUnitEquivalentID,
                        principalTable: "ParameterUnitEquivalents",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GradePrefix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GradeValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsSizeApplicable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasters_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCaseAliasNames",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceConfigurationID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCaseAliasNames", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceCaseAliasNames_InvoiceCaseConfigurations_InvoiceConfigurationID",
                        column: x => x.InvoiceConfigurationID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablCompetenceRequirements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<long>(type: "bigint", nullable: true),
                    PositionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinimumEducation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumExperience = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    RelatedActivity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCompetenceRequirements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCompetenceRequirements_DesignationMasters_PositionId",
                        column: x => x.PositionId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablResponsibilityAuthorities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Responsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Authorities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeAccepted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptanceTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablResponsibilityAuthorities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablResponsibilityAuthorities_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablSkillMatrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Decision = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SkillsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSkillsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSkillMatrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablSkillMatrices_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablSkillMatrixDecisions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablSkillMatrixDecisions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablSkillMatrixDecisions_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationMetalClassifications",
                columns: table => new
                {
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenOrientationMetalClassifications", x => new { x.SpecimenOrientationID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationMetalClassifications_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenOrientationProductForms",
                columns: table => new
                {
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: false),
                    ProductFormID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenOrientationProductForms", x => new { x.SpecimenOrientationID, x.ProductFormID });
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationProductForms_ProductFormMasters_ProductFormID",
                        column: x => x.ProductFormID,
                        principalTable: "ProductFormMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecimenOrientationProductForms_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuttingPriceVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingPriceMasterID = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    RatePerUnit = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    RatePerUnitHard = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingPriceVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingPriceVersions_CuttingPriceMasters_CuttingPriceMasterID",
                        column: x => x.CuttingPriceMasterID,
                        principalTable: "CuttingPriceMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CuttingPriceVersions_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpecificationGrades",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsUNS = table.Column<bool>(type: "bit", nullable: true),
                    UNSSteelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentifierValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationGrades", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationGrades_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationGrades_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupersededDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StandardFile = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StandardFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationVersions_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: false),
                    EntityID = table.Column<long>(type: "bigint", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStepID = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_WorkflowSteps_CurrentStepID",
                        column: x => x.CurrentStepID,
                        principalTable: "WorkflowSteps",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepID = table.Column<long>(type: "bigint", nullable: false),
                    ToStepID = table.Column<long>(type: "bigint", nullable: true),
                    ToStepName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTransitions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_WorkflowSteps_StepID",
                        column: x => x.StepID,
                        principalTable: "WorkflowSteps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimensionalFactorProductForms",
                columns: table => new
                {
                    DimensionalFactorID = table.Column<long>(type: "bigint", nullable: false),
                    ProductFormID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionalFactorProductForms", x => new { x.DimensionalFactorID, x.ProductFormID });
                    table.ForeignKey(
                        name: "FK_DimensionalFactorProductForms_DimensionalFactorMasters_DimensionalFactorID",
                        column: x => x.DimensionalFactorID,
                        principalTable: "DimensionalFactorMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimensionalFactorProductForms_ProductFormMasters_ProductFormID",
                        column: x => x.ProductFormID,
                        principalTable: "ProductFormMasters",
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
                name: "ProductConditionPropertyTypes",
                columns: table => new
                {
                    ProductConditionID = table.Column<long>(type: "bigint", nullable: false),
                    PropertyTypeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConditionPropertyTypes", x => new { x.ProductConditionID, x.PropertyTypeID });
                    table.ForeignKey(
                        name: "FK_ProductConditionPropertyTypes_ProductConditionMasters_ProductConditionID",
                        column: x => x.ProductConditionID,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductConditionPropertyTypes_PropertyTypeMasters_PropertyTypeID",
                        column: x => x.PropertyTypeID,
                        principalTable: "PropertyTypeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LabDepartmentID = table.Column<long>(type: "bigint", nullable: true),
                    Equation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChemicalTest = table.Column<bool>(type: "bit", nullable: false),
                    IsMechanical = table.Column<bool>(type: "bit", nullable: false),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: true),
                    TestDuration = table.Column<int>(type: "int", nullable: true),
                    GlobalUsageCount = table.Column<int>(type: "int", nullable: false),
                    RecentUsageCount = table.Column<int>(type: "int", nullable: false),
                    LastPerformedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTests_DepartmentMasters_LabDepartmentID",
                        column: x => x.LabDepartmentID,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTests_DisciplineMasters_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "DisciplineMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LabRooms",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DepartmentID = table.Column<long>(type: "bigint", nullable: true),
                    Building = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultTempMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    DefaultTempMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    DefaultHumidityMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    DefaultHumidityMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    MonitoringEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabRooms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabRooms_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabRooms_DepartmentMasters_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablJobDescriptions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationId = table.Column<long>(type: "bigint", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportingTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinimumQualification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TechnicalTraining = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Experience = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrincipalAccountabilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorityStopTesting = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityIssueReports = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityAccessConfidential = table.Column<bool>(type: "bit", nullable: false),
                    AuthorityEquipmentCalibration = table.Column<bool>(type: "bit", nullable: false),
                    QmsResponsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfidentialityClause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreparedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmployeeAccepted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablJobDescriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablJobDescriptions_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NablJobDescriptions_DesignationMasters_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "DesignationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablPurchaseIndents",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPurchaseIndents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablPurchaseIndents_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablQualityControlPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ControlType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FrequencyUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPerformedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionOnFailure = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablQualityControlPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablQualityControlPlans_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CuttingChargeHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingChargeHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingChargeHeaders_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InwardAddresses",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactPersonID = table.Column<long>(type: "bigint", nullable: true),
                    ContactPersonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InwardAddresses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InwardAddresses_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InwardAddresses_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InwardContacts",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Selected = table.Column<bool>(type: "bit", nullable: false),
                    ContactID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendBill = table.Column<bool>(type: "bit", nullable: false),
                    SendReport = table.Column<bool>(type: "bit", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InwardContacts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InwardContacts_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InwardDispatchModes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    DispatchModeID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InwardDispatchModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InwardDispatchModes_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceHeader",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    PINo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsGenerated = table.Column<bool>(type: "bit", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceHeader", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeader_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeader_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuotationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcceptedBy = table.Column<long>(type: "bigint", nullable: true),
                    AcceptedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedToInwardID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Quotations_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Quotations_SampleInwards_ConvertedToInwardID",
                        column: x => x.ConvertedToInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TaxInvoices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SACCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CurrencySymbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ForeignCurrencyGrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxInvoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TaxInvoices_CustomerPurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "CustomerPurchaseOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxInvoices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxInvoices_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaxInvoices_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetalClassificationParameters",
                columns: table => new
                {
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetalClassificationParameters", x => new { x.MetalClassificationID, x.ParameterID });
                    table.ForeignKey(
                        name: "FK_MetalClassificationParameters_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MetalClassificationParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterDropdownOptions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterDropdownOptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterDropdownOptions_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterSpecimenOrientation",
                columns: table => new
                {
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterSpecimenOrientation", x => new { x.ParameterID, x.SpecimenOrientationID });
                    table.ForeignKey(
                        name: "FK_ParameterSpecimenOrientation_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterSpecimenOrientation_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationHeaderParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitEquivalentID = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationHeaderParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                        column: x => x.ParameterUnitEquivalentID,
                        principalTable: "ParameterUnitEquivalents",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationHeaderParameters_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestMethodSpecificationParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitEquivalentID = table.Column<long>(type: "bigint", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSpecificationParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationParameters_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestMethodSpecificationParameters_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToleranceMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterID = table.Column<long>(type: "bigint", nullable: true),
                    StandardName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValueRangeStart = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ValueRangeEnd = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Tolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ToleranceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToleranceMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToleranceMasters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ToleranceMasters_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterMetalClassifications",
                columns: table => new
                {
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterMetalClassifications", x => new { x.ProductMasterID, x.MetalClassificationID });
                    table.ForeignKey(
                        name: "FK_ProductMasterMetalClassifications_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterMetalClassifications_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: false),
                    VersionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SpecificationFilePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    StandardOrganizationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecStdNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PartSection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProductCaption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActiveVersion = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterVersions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersions_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersions_StandardOrganizationMasters_StandardOrganizationID",
                        column: x => x.StandardOrganizationID,
                        principalTable: "StandardOrganizationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SampleDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: true),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID = table.Column<long>(type: "bigint", nullable: true),
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: true),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: true),
                    IsUnknownSample = table.Column<bool>(type: "bit", nullable: false),
                    AssignedGradeID = table.Column<long>(type: "bigint", nullable: true),
                    AssignedGradeNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: true),
                    ProductFormID = table.Column<long>(type: "bigint", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledBy = table.Column<long>(type: "bigint", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TpiRequired = table.Column<bool>(type: "bit", nullable: false),
                    TpiAgencyID = table.Column<long>(type: "bigint", nullable: true),
                    TpiInspectorsJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Specimen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    SampleFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReportUnlocked = table.Column<bool>(type: "bit", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    IsTestingCompleted = table.Column<bool>(type: "bit", nullable: false),
                    TestingCompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Thickness = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Diameter = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleDetails_DisciplineMasters_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "DisciplineMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_ProductConditionMasters_ProductConditionID",
                        column: x => x.ProductConditionID,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_ProductFormMasters_ProductFormID",
                        column: x => x.ProductFormID,
                        principalTable: "ProductFormMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleDetails_SpecificationGrades_AssignedGradeID",
                        column: x => x.AssignedGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SampleDetails_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SpecificationVersionParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationVersionID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationVersionParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationVersionParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationVersionParameters_SpecificationVersions_SpecificationVersionID",
                        column: x => x.SpecificationVersionID,
                        principalTable: "SpecificationVersions",
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

            migrationBuilder.CreateTable(
                name: "LaboratoryTestConditions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ConditionMasterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestConditions_ConditionMasters_ConditionMasterID",
                        column: x => x.ConditionMasterID,
                        principalTable: "ConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestConditions_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestMethods_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestParameters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReportTestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TestDuration = table.Column<int>(type: "int", nullable: true),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroups_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroups_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LabScopeMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScopeRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BranchID = table.Column<long>(type: "bigint", nullable: true),
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
                        name: "FK_LabScopeMasters_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabScopeMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportFormatMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFormatID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportFormatMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportFormatMappings_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReportFormatMappings_ReportFormats_ReportFormatID",
                        column: x => x.ReportFormatID,
                        principalTable: "ReportFormats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportFormatMappings_TestMethodSpecifications_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TestType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestTypeID = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FormatType = table.Column<int>(type: "int", nullable: false),
                    HeaderConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionVisibilityJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatoryConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageLayout = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ShowCalibrationTable = table.Column<bool>(type: "bit", nullable: false),
                    ShowSpecificationRow = table.Column<bool>(type: "bit", nullable: false),
                    ShowNablColumn = table.Column<bool>(type: "bit", nullable: false),
                    WatermarkText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportTemplates_LaboratoryTests_TestTypeID",
                        column: x => x.TestTypeID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SamplePreparationMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodStandardID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dimensions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaterialType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplePreparationMasters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SamplePreparationMasters_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparationMasters_TestMethodSpecifications_TestMethodStandardID",
                        column: x => x.TestMethodStandardID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SpecificationLines",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationVersionID = table.Column<long>(type: "bigint", nullable: false),
                    ManualSelection = table.Column<bool>(type: "bit", nullable: true),
                    ParameterID = table.Column<long>(type: "bigint", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Equation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: true),
                    ParameterUnitEquivalentID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    MinEquation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxEquation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValueEquation = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaxValueEquation = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MinTolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaxTolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SpecimenOrientationID = table.Column<long>(type: "bigint", nullable: true),
                    DimensionalFactorID = table.Column<long>(type: "bigint", nullable: true),
                    LowerLimitValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperLimitValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerLimitDecimalValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    UpperLimitDecimalValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID1 = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID2 = table.Column<long>(type: "bigint", nullable: true),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    TestCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_SpecificationLines_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ParameterUnitEquivalents_ParameterUnitEquivalentID",
                        column: x => x.ParameterUnitEquivalentID,
                        principalTable: "ParameterUnitEquivalents",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ParameterUnitMasters_ParameterUnitID",
                        column: x => x.ParameterUnitID,
                        principalTable: "ParameterUnitMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_SpecificationVersions_SpecificationVersionID",
                        column: x => x.SpecificationVersionID,
                        principalTable: "SpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLines_SpecimenOrientationMasters_SpecimenOrientationID",
                        column: x => x.SpecimenOrientationID,
                        principalTable: "SpecimenOrientationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TestGroupMappings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestGroupID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroupMappings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestGroupMappings_LaboratoryTests_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestGroupMappings_TestGroups_TestGroupID",
                        column: x => x.TestGroupID,
                        principalTable: "TestGroups",
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
                    SampleSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestMethodSubGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestMethodSubGroups_LaboratoryTests_TestMethodID",
                        column: x => x.TestMethodID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TestUsageStats",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID = table.Column<long>(type: "bigint", nullable: true),
                    CustomerID = table.Column<long>(type: "bigint", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    RecentUsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComputedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUsageStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestUsageStats_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMasters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EquipmentNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentTypeID = table.Column<long>(type: "bigint", nullable: false),
                    OEMID = table.Column<long>(type: "bigint", nullable: false),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalibrationRequired = table.Column<bool>(type: "bit", nullable: false),
                    NextCalibrationDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceRequired = table.Column<bool>(type: "bit", nullable: false),
                    MaintenanceInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextMaintenanceDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InternalExternal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntermediateCheckRequired = table.Column<bool>(type: "bit", nullable: false),
                    IntermediateCheckInterval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabRoomID = table.Column<long>(type: "bigint", nullable: true),
                    LastCalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationFrequencyDays = table.Column<int>(type: "int", nullable: true),
                    MaintenanceSchedule = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                        name: "FK_EquipmentMasters_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentMasters_EquipmentTypeMasters_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalTable: "EquipmentTypeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentMasters_LabRooms_LabRoomID",
                        column: x => x.LabRoomID,
                        principalTable: "LabRooms",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablEnvironmentMonitorings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LabRoomId = table.Column<long>(type: "bigint", nullable: true),
                    RoomName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MonitoringMonth = table.Column<int>(type: "int", nullable: true),
                    MonitoringYear = table.Column<int>(type: "int", nullable: true),
                    MonitoringDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeOfReading = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableTemperatureMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableTemperatureMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableHumidityMin = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AcceptableHumidityMax = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    IsWithinLimits = table.Column<bool>(type: "bit", nullable: false),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEnvironmentMonitorings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEnvironmentMonitorings_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablEnvironmentMonitorings_LabRooms_LabRoomId",
                        column: x => x.LabRoomId,
                        principalTable: "LabRooms",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablPurchaseOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PurchaseIndentId = table.Column<long>(type: "bigint", nullable: true),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPurchaseOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablPurchaseOrders_NablPurchaseIndents_PurchaseIndentId",
                        column: x => x.PurchaseIndentId,
                        principalTable: "NablPurchaseIndents",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablPurchaseOrders_SupplierMasters_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CuttingChargeSamples",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingChargeHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    SpecimenTypeId = table.Column<long>(type: "bigint", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Thickness = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Diameter = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Orientation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreparationInstructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreparationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SampleTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingChargeSamples", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingChargeSamples_CuttingChargeHeaders_CuttingChargeHeaderID",
                        column: x => x.CuttingChargeHeaderID,
                        principalTable: "CuttingChargeHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuttingChargeSamples_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CuttingChargeSamples_SpecimenTypeMasters_SpecimenTypeId",
                        column: x => x.SpecimenTypeId,
                        principalTable: "SpecimenTypeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProformaInvoiceHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    ChargeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                        column: x => x.ProformaInvoiceHeaderID,
                        principalTable: "ProformaInvoiceHeader",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationID = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuotationItems_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvancePaymentVouchers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdjustedToInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    AdjustedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancePaymentVouchers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AdvancePaymentVouchers_TaxInvoices_AdjustedToInvoiceID",
                        column: x => x.AdjustedToInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CreditNotes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditNoteNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreditNoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CreditNotes_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CreditNotes_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditNotes_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CustomerLedgers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InwardId = table.Column<long>(type: "bigint", nullable: true),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerLedgers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerLedgers_SampleInwards_InwardId",
                        column: x => x.InwardId,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerLedgers_TaxInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebitNotes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DebitNoteNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DebitNoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerID = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DebitNotes_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DebitNotes_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DebitNotes_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLineItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProformaInvoiceHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLineItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_ProformaInvoiceHeader_ProformaInvoiceHeaderID",
                        column: x => x.ProformaInvoiceHeaderID,
                        principalTable: "ProformaInvoiceHeader",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: true),
                    CaseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleID = table.Column<long>(type: "bigint", nullable: true),
                    ReportID = table.Column<long>(type: "bigint", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazorpayOrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazorpayPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RazorpaySignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    WhatsAppSent = table.Column<bool>(type: "bit", nullable: false),
                    SMSSent = table.Column<bool>(type: "bit", nullable: false),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    LinkSentOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterVersionGrades",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterVersionID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterVersionGrades", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGrades_ProductMasterVersions_ProductMasterVersionID",
                        column: x => x.ProductMasterVersionID,
                        principalTable: "ProductMasterVersions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGrades_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChargeEvents",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: true),
                    ReportID = table.Column<long>(type: "bigint", nullable: true),
                    ChargeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    ProformaInvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    SelectionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: true),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoicedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeEvents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChargeEvents_ProformaInvoiceHeader_ProformaInvoiceID",
                        column: x => x.ProformaInvoiceID,
                        principalTable: "ProformaInvoiceHeader",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChargeEvents_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChargeEvents_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargeEvents_TaxInvoices_TaxInvoiceID",
                        column: x => x.TaxInvoiceID,
                        principalTable: "TaxInvoices",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "GeneratedReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFormatID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateNo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_ReportFormats_ReportFormatID",
                        column: x => x.ReportFormatID,
                        principalTable: "ReportFormats",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_GeneratedReports_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MachiningChargeItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MachiningChargeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachiningChargeItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MachiningChargeItems_MachiningChargeMasters_MachiningChargeMasterID",
                        column: x => x.MachiningChargeMasterID,
                        principalTable: "MachiningChargeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_MachiningChargeItems_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowInstanceId = table.Column<long>(type: "bigint", nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportHeaders_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SampleAdditionalDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleAdditionalDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleAdditionalDetails_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ReplanCount = table.Column<int>(type: "int", nullable: false),
                    PlanStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestPlans_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultHeaders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestPlanID = table.Column<long>(type: "bigint", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    TestID = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsOverallPass = table.Column<bool>(type: "bit", nullable: true),
                    IsNabl = table.Column<bool>(type: "bit", nullable: false),
                    LabNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedBy = table.Column<long>(type: "bigint", nullable: true),
                    LabRoomId = table.Column<long>(type: "bigint", nullable: true),
                    RoomTemperature = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    RoomHumidity = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    EquipmentIdsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TestEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerformedById = table.Column<long>(type: "bigint", nullable: true),
                    CalculatedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OverridePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OverrideById = table.Column<long>(type: "bigint", nullable: true),
                    PriceOverridden = table.Column<bool>(type: "bit", nullable: false),
                    SelectedPricingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PricingDimensionValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparationDataMissing = table.Column<bool>(type: "bit", nullable: false),
                    OrientationDeviationAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultHeaders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultHeaders_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResultHeaders_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TpiInspections",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: false),
                    SampleDetailID = table.Column<long>(type: "bigint", nullable: true),
                    TPIMasterID = table.Column<long>(type: "bigint", nullable: false),
                    Stage = table.Column<string>(type: "varchar(30)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InspectorComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TpiInspections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TpiInspections_SampleDetails_SampleDetailID",
                        column: x => x.SampleDetailID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TpiInspections_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TpiInspections_TPIMasters_TPIMasterID",
                        column: x => x.TPIMasterID,
                        principalTable: "TPIMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TestDuration = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypes_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypes_MetalClassificationMasters_MetalClassificationID",
                        column: x => x.MetalClassificationID,
                        principalTable: "MetalClassificationMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupInvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupInvoiceCases_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupInvoiceCases_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupMethods_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupParameters_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: true),
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupSpecifications_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LabScopeSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecifications_LabScopeMasters_LabScopeID",
                        column: x => x.LabScopeID,
                        principalTable: "LabScopeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecifications_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LabScopeSpecifications_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplateBlocks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportTemplateID = table.Column<long>(type: "bigint", nullable: false),
                    BlockType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConditionExpression = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplateBlocks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportTemplateBlocks_ReportTemplates_ReportTemplateID",
                        column: x => x.ReportTemplateID,
                        principalTable: "ReportTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationLineConditions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: false),
                    ConditionMasterID = table.Column<long>(type: "bigint", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLineConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationLineConditions_ConditionMasters_ConditionMasterID",
                        column: x => x.ConditionMasterID,
                        principalTable: "ConditionMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecificationLineConditions_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationLineTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    NumberOfTestSpecimen = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationLineTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpecificationLineTestMethods_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SpecificationLineTestMethods_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecificationLineTestMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentAnalysisTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAnalysisTechniques", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentAnalysisTechniques_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_EquipmentAnalysisTechniques_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCalibration",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    CalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CalibrationDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CalibrationAgencyID = table.Column<long>(type: "bigint", nullable: true),
                    Agency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCalibration", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentCalibration_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMaintenance",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    MaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMaintenance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentMaintenance_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentReferenceMaterials",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LotNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentReferenceMaterials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentReferenceMaterials_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSOP",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSOP", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EquipmentSOP_EquipmentMasters_EquipmentMasterID",
                        column: x => x.EquipmentMasterID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestSubGroupEquipments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestSubGroupEquipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupEquipments_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestSubGroupEquipments_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablCalibrationReviews",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EquipmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationAgencyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CalibrationDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CalibrationDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewConclusion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablCalibrationReviews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablCalibrationReviews_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablEquipmentHistories",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EquipmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalibrationFrequency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastCalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextCalibrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CalibrationAgency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaintenanceRecordsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEquipmentHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEquipmentHistories_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablIntermediateChecks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenceStandard = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ObservedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    AcceptedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Tolerance = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ResultStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablIntermediateChecks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablIntermediateChecks_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTechnicalRawDatas",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TestParameter = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    ObservationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservationsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedResult = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Uncertainty = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RawDataFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTechnicalRawDatas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTechnicalRawDatas_EquipmentMasters_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentDailyRecords",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvironmentMonitoringID = table.Column<long>(type: "bigint", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Pressure = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    AirQuality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservedTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ObservedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsWithinLimits = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentDailyRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EnvironmentDailyRecords_NablEnvironmentMonitorings_EnvironmentMonitoringID",
                        column: x => x.EnvironmentMonitoringID,
                        principalTable: "NablEnvironmentMonitorings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablPurchaseMaterialVerifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    PONumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsVerificationJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GRNNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablPurchaseMaterialVerifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablPurchaseMaterialVerifications_NablPurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "NablPurchaseOrders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CuttingChargeDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingChargeSampleID = table.Column<long>(type: "bigint", nullable: false),
                    CuttingID = table.Column<long>(type: "bigint", nullable: false),
                    CuttingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingChargeDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuttingChargeDetails_CuttingChargeSamples_CuttingChargeSampleID",
                        column: x => x.CuttingChargeSampleID,
                        principalTable: "CuttingChargeSamples",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuttingChargeDetails_CuttingPriceMasters_CuttingID",
                        column: x => x.CuttingID,
                        principalTable: "CuttingPriceMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMasterVersionGradeConditions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductMasterVersionGradeID = table.Column<long>(type: "bigint", nullable: false),
                    ProductConditionID1 = table.Column<long>(type: "bigint", nullable: true),
                    ProductConditionID2 = table.Column<long>(type: "bigint", nullable: true),
                    HeatTreatmentID = table.Column<long>(type: "bigint", nullable: true),
                    ProductSizeMasterID = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasterVersionGradeConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_HeatTreatmentMasters_HeatTreatmentID",
                        column: x => x.HeatTreatmentID,
                        principalTable: "HeatTreatmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductConditionMasters_ProductConditionID1",
                        column: x => x.ProductConditionID1,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductConditionMasters_ProductConditionID2",
                        column: x => x.ProductConditionID2,
                        principalTable: "ProductConditionMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductMasterVersionGrades_ProductMasterVersionGradeID",
                        column: x => x.ProductMasterVersionGradeID,
                        principalTable: "ProductMasterVersionGrades",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductMasterVersionGradeConditions_ProductSizeMasters_ProductSizeMasterID",
                        column: x => x.ProductSizeMasterID,
                        principalTable: "ProductSizeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AmendmentRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmendmentRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AmendmentRequests_ReportHeaders_ReportHeaderID",
                        column: x => x.ReportHeaderID,
                        principalTable: "ReportHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdfPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAmendmentAllowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reports_ReportHeaders_ReportHeaderID",
                        column: x => x.ReportHeaderID,
                        principalTable: "ReportHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleTestPlanID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestSubGroupID = table.Column<long>(type: "bigint", nullable: true),
                    Specification1 = table.Column<long>(type: "bigint", nullable: true),
                    Specification2 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GeneralTests_LaboratoryTestSubGroups_LaboratoryTestSubGroupID",
                        column: x => x.LaboratoryTestSubGroupID,
                        principalTable: "LaboratoryTestSubGroups",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_GeneralTests_TestPlans_SampleTestPlanID",
                        column: x => x.SampleTestPlanID,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedById = table.Column<long>(type: "bigint", nullable: false),
                    ChangedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FieldChangesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanHistories_TestPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplanRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedById = table.Column<long>(type: "bigint", nullable: false),
                    RequestedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplanRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplanRequests_TestPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversalTestGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    SampleTestPlanID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationVersionID = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalTestGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_SpecificationVersions_SpecificationVersionID",
                        column: x => x.SpecificationVersionID,
                        principalTable: "SpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UniversalTestGroups_TestPlans_SampleTestPlanID",
                        column: x => x.SampleTestPlanID,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LongTermTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    DurationHours = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LongTermTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LongTermTests_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LongTermTests_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultImages",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultImages_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResultParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultHeaderID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    IsAdditional = table.Column<bool>(type: "bit", nullable: false),
                    IsCalculated = table.Column<bool>(type: "bit", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    IsWithinLimit = table.Column<bool>(type: "bit", nullable: true),
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: true),
                    FormulaExpression = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DependsOnParamsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecMinValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SpecMaxValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsStandalone = table.Column<bool>(type: "bit", nullable: false),
                    SourceTestMethodId = table.Column<long>(type: "bigint", nullable: true),
                    IsBillable = table.Column<bool>(type: "bit", nullable: false),
                    ResultStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsWithinNablScope = table.Column<bool>(type: "bit", nullable: true),
                    NablScopeStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LabScopeSpecParamId = table.Column<long>(type: "bigint", nullable: true),
                    ExpandedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CoverageFactor = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CombinedUncertainty = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    NablMeasurementUncertaintyId = table.Column<long>(type: "bigint", nullable: true),
                    ParameterType = table.Column<string>(type: "varchar(20)", nullable: true),
                    TestMethodUsed = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DecimalPrecision = table.Column<int>(type: "int", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ConvertedValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SelectedUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestResultParameters_TestResultHeaders_TestResultHeaderID",
                        column: x => x.TestResultHeaderID,
                        principalTable: "TestResultHeaders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChemicalTests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleTestPlanID = table.Column<long>(type: "bigint", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UlrNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    Specification1 = table.Column<long>(type: "bigint", nullable: true),
                    Specification2 = table.Column<long>(type: "bigint", nullable: true),
                    TestMethod = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTests_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChemicalTests_TestPlans_SampleTestPlanID",
                        column: x => x.SampleTestPlanID,
                        principalTable: "TestPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTypeID = table.Column<long>(type: "bigint", nullable: true),
                    DefaultPricingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceCases_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceCases_LaboratoryTestAnalysisTypes_AnalysisTypeID",
                        column: x => x.AnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InvoiceCases_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeEquipments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeEquipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeEquipments_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeEquipments_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeInvoiceCases",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeInvoiceCases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeInvoiceCases_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeInvoiceCases_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    TestMethodSpecificationVersionID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeMethods_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationVersions_TestMethodSpecificationVersionID",
                        column: x => x.TestMethodSpecificationVersionID,
                        principalTable: "TestMethodSpecificationVersions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsReportable = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeParameters_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeParameters_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeSpecifications",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationHeaderID = table.Column<long>(type: "bigint", nullable: true),
                    SpecificationGradeID = table.Column<long>(type: "bigint", nullable: true),
                    ProductMasterID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeSpecifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_ProductMasters_ProductMasterID",
                        column: x => x.ProductMasterID,
                        principalTable: "ProductMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_SpecificationGrades_SpecificationGradeID",
                        column: x => x.SpecificationGradeID,
                        principalTable: "SpecificationGrades",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeSpecifications_SpecificationHeaders_SpecificationHeaderID",
                        column: x => x.SpecificationHeaderID,
                        principalTable: "SpecificationHeaders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryTestAnalysisTypeTechniques",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisTechniqueID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryTestAnalysisTypeTechniques", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeTechniques_AnalysisTechniqueMasters_AnalysisTechniqueID",
                        column: x => x.AnalysisTechniqueID,
                        principalTable: "AnalysisTechniqueMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LaboratoryTestAnalysisTypeTechniques_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabScopeSpecificationParameters",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeSpecificationID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: false),
                    QualitativeQuantitative = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUnderISO = table.Column<bool>(type: "bit", nullable: false),
                    LowerLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerLimitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpperLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperLimitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DisciplineID = table.Column<long>(type: "bigint", nullable: true),
                    GroupID = table.Column<long>(type: "bigint", nullable: true),
                    SubGroupID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeSpecificationParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecificationParameters_LabScopeSpecifications_LabScopeSpecificationID",
                        column: x => x.LabScopeSpecificationID,
                        principalTable: "LabScopeSpecifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportAmendmentTokens",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportID = table.Column<long>(type: "bigint", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    LinkExpiryOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FreeUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportAmendmentTokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportAmendmentTokens_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportBlocks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportID = table.Column<long>(type: "bigint", nullable: false),
                    BlockType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportBlocks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportBlocks_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneralTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: false),
                    StandardID = table.Column<long>(type: "bigint", nullable: false),
                    TestCaseID = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UlrNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cancel = table.Column<bool>(type: "bit", nullable: false),
                    PreparationRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GeneralTestMethods_GeneralTests_GeneralTestID",
                        column: x => x.GeneralTestID,
                        principalTable: "GeneralTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestExecutions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversalTestGroupID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: false),
                    ExecutionAnalystID = table.Column<long>(type: "bigint", nullable: true),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutionNo = table.Column<int>(type: "int", nullable: false),
                    IsRetest = table.Column<bool>(type: "bit", nullable: false),
                    PreviousExecutionID = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExecutionConfigSnapshotID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestExecutions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestExecutions_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestExecutions_ExecutionConfigSnapshots_ExecutionConfigSnapshotID",
                        column: x => x.ExecutionConfigSnapshotID,
                        principalTable: "ExecutionConfigSnapshots",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestExecutions_Organizations_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestExecutions_TestExecutions_PreviousExecutionID",
                        column: x => x.PreviousExecutionID,
                        principalTable: "TestExecutions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestExecutions_UniversalTestGroups_UniversalTestGroupID",
                        column: x => x.UniversalTestGroupID,
                        principalTable: "UniversalTestGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LongTermRecords",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LongTermTestID = table.Column<long>(type: "bigint", nullable: false),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LongTermRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LongTermRecords_LongTermTests_LongTermTestID",
                        column: x => x.LongTermTestID,
                        principalTable: "LongTermTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChemicalTestElements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemicalTestID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationLineID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParameterUnitID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Selected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTestElements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTestElements_ChemicalTests_ChemicalTestID",
                        column: x => x.ChemicalTestID,
                        principalTable: "ChemicalTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChemicalTestElements_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChemicalTestElements_ParameterMasters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChemicalTestElements_SpecificationLines_SpecificationLineID",
                        column: x => x.SpecificationLineID,
                        principalTable: "SpecificationLines",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChemicalTestMethods",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemicalTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: false),
                    TestMethodSpecificationID = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UlrNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cancel = table.Column<bool>(type: "bit", nullable: false),
                    PreparationRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTestMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTestMethods_ChemicalTests_ChemicalTestID",
                        column: x => x.ChemicalTestID,
                        principalTable: "ChemicalTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChemicalTestMethods_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChemicalTestMethods_TestMethodSpecifications_TestMethodSpecificationID",
                        column: x => x.TestMethodSpecificationID,
                        principalTable: "TestMethodSpecifications",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChemicalTestTypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemicalTestID = table.Column<long>(type: "bigint", nullable: false),
                    LaboratoryTestID = table.Column<long>(type: "bigint", nullable: true),
                    LaboratoryTestAnalysisTypeID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTestTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTestTypes_ChemicalTests_ChemicalTestID",
                        column: x => x.ChemicalTestID,
                        principalTable: "ChemicalTests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChemicalTestTypes_LaboratoryTestAnalysisTypes_LaboratoryTestAnalysisTypeID",
                        column: x => x.LaboratoryTestAnalysisTypeID,
                        principalTable: "LaboratoryTestAnalysisTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChemicalTestTypes_LaboratoryTests_LaboratoryTestID",
                        column: x => x.LaboratoryTestID,
                        principalTable: "LaboratoryTests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCasePrices",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCaseID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AliasName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceCaseConfigID = table.Column<long>(type: "bigint", nullable: false),
                    ElementPrices = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCasePrices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceCasePrices_InvoiceCaseConfigurations_InvoiceCaseConfigID",
                        column: x => x.InvoiceCaseConfigID,
                        principalTable: "InvoiceCaseConfigurations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceCasePrices_InvoiceCases_InvoiceCaseID",
                        column: x => x.InvoiceCaseID,
                        principalTable: "InvoiceCases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabScopeSpecificationParameterEquipments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabScopeSpecificationParameterID = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabScopeSpecificationParameterEquipments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LabScopeSpecificationParameterEquipments_LabScopeSpecificationParameters_LabScopeSpecificationParameterID",
                        column: x => x.LabScopeSpecificationParameterID,
                        principalTable: "LabScopeSpecificationParameters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAmendments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportID = table.Column<long>(type: "bigint", nullable: false),
                    TokenID = table.Column<long>(type: "bigint", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsChargeable = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentOrderID = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAmendments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerAmendments_ReportAmendmentTokens_TokenID",
                        column: x => x.TokenID,
                        principalTable: "ReportAmendmentTokens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAmendments_Reports_ReportID",
                        column: x => x.ReportID,
                        principalTable: "Reports",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSpecimens",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestExecutionID = table.Column<long>(type: "bigint", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    SpecimenIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDiscarded = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSpecimens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestSpecimens_TestExecutions_TestExecutionID",
                        column: x => x.TestExecutionID,
                        principalTable: "TestExecutions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestObservations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestSpecimenID = table.Column<long>(type: "bigint", nullable: false),
                    ReadingNo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestObservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestObservations_TestSpecimens_TestSpecimenID",
                        column: x => x.TestSpecimenID,
                        principalTable: "TestSpecimens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterObservationResults",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestObservationID = table.Column<long>(type: "bigint", nullable: false),
                    ParameterMasterID = table.Column<long>(type: "bigint", nullable: false),
                    RawValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CalculatedValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsFormulaCalculated = table.Column<bool>(type: "bit", nullable: false),
                    SpecMin = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    SpecMax = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ResultStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterObservationResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParameterObservationResults_ParameterMasters_ParameterMasterID",
                        column: x => x.ParameterMasterID,
                        principalTable: "ParameterMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterObservationResults_TestObservations_TestObservationID",
                        column: x => x.TestObservationID,
                        principalTable: "TestObservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_EmployeeDocuments_UploadFiles_UploadReferenceID",
                        column: x => x.UploadReferenceID,
                        principalTable: "UploadFiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                    BranchID = table.Column<long>(type: "bigint", nullable: true),
                    ReportingManagerID = table.Column<long>(type: "bigint", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    RoleID = table.Column<long>(type: "bigint", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTeamHead = table.Column<bool>(type: "bit", nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualificationSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Experience = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingRecordsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetencyLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSystemAdmin = table.Column<bool>(type: "bit", nullable: false),
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
                        name: "FK_EmployeeMasters_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_EmployeeMasters_EmployeeMasters_ReportingManagerID",
                        column: x => x.ReportingManagerID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
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
                name: "NablAuditPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditYear = table.Column<int>(type: "int", nullable: true),
                    AuditType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Period = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AreaDepartment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditScheduleJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditObjective = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeadAuditorId = table.Column<long>(type: "bigint", nullable: true),
                    LeadAuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablAuditPlans_EmployeeMasters_LeadAuditorId",
                        column: x => x.LeadAuditorId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeeAuthorizations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    PersonnelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Uid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestMethodAuthorization = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestAuthorization = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeAuthorizations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEmployeeAuthorizations_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablEmployeeAuthorizations_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeeCompetences",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EvaluationPeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationPeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpecificTrainingRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EvaluationDoneBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeeCompetences", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEmployeeCompetences_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablEmployeePerformanceRecords",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReviewPeriod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TechnicalRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    BehavioralRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    OverallRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablEmployeePerformanceRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablEmployeePerformanceRecords_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NablInductionTrainings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainerDesignation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SampleRefNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Parameter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestMethodSop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationMode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EvalParameter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvalTestMethodSop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ObservedValue1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservedValue2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservedValueAverage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainerComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablInductionTrainings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablInductionTrainings_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablInternalAuditors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LeadAuditorCourse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LeadAuditorCertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InternalAuditorCourse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InternalAuditorCertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ISOClauses = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AuditExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizedAreas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorizationValidUpto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablInternalAuditors", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablInternalAuditors_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablMeetingAgendas",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeetingVenue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChairpersonId = table.Column<long>(type: "bigint", nullable: true),
                    ChairpersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AgendaItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttendeeIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttendeeNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousMOMRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMeetingAgendas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablMeetingAgendas_EmployeeMasters_ChairpersonId",
                        column: x => x.ChairpersonId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTrainingPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanningYear = table.Column<int>(type: "int", nullable: true),
                    PlanDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CoursesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingTopic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingObjective = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainerDesignation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VenueMode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NeedIdentifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompletionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTrainingPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTrainingPlans_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SamplePreparations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    InwardID = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AssignedToEmployeeID = table.Column<long>(type: "bigint", nullable: true),
                    PreparedByEmployeeID = table.Column<long>(type: "bigint", nullable: true),
                    VerifiedByEmployeeID = table.Column<long>(type: "bigint", nullable: true),
                    AssignedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EquipmentID = table.Column<long>(type: "bigint", nullable: true),
                    PreparationMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    PreparationInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PreConditionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PostConditionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VerificationRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CuttingChargesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MachiningChargesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherChargesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplePreparations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SamplePreparations_EmployeeMasters_AssignedToEmployeeID",
                        column: x => x.AssignedToEmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparations_EmployeeMasters_PreparedByEmployeeID",
                        column: x => x.PreparedByEmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparations_EmployeeMasters_VerifiedByEmployeeID",
                        column: x => x.VerifiedByEmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparations_EquipmentMasters_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "EquipmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparations_SampleDetails_SampleID",
                        column: x => x.SampleID,
                        principalTable: "SampleDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SamplePreparations_SampleInwards_InwardID",
                        column: x => x.InwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
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
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsLoginEnabled = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationID = table.Column<long>(type: "bigint", nullable: true),
                    BranchID = table.Column<long>(type: "bigint", nullable: true),
                    CanViewAllBranches = table.Column<bool>(type: "bit", nullable: false),
                    RemoteLogin = table.Column<bool>(type: "bit", nullable: false),
                    IpRestriction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkingHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    SessionTimeout = table.Column<int>(type: "int", nullable: false),
                    ForcePasswordChange = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorOtp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorOtpExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwoFactorLastSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwoFactorSendCount = table.Column<int>(type: "int", nullable: false),
                    AutoLockAfterAttempts = table.Column<int>(type: "int", nullable: false),
                    UnlockMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_UserMasters_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMasters_EmployeeMasters_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UserMasters_RoleMasters_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMasters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowActionLogs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowID = table.Column<long>(type: "bigint", nullable: false),
                    InstanceID = table.Column<long>(type: "bigint", nullable: false),
                    StepID = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeID = table.Column<long>(type: "bigint", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowActionLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkflowActionLogs_EmployeeMasters_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowActionLogs_WorkflowSteps_StepID",
                        column: x => x.StepID,
                        principalTable: "WorkflowSteps",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkflowActionLogs_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablAuditChecklists",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditPlanId = table.Column<long>(type: "bigint", nullable: true),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditorId = table.Column<long>(type: "bigint", nullable: true),
                    AuditorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditeeId = table.Column<long>(type: "bigint", nullable: true),
                    AuiteeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ISOClause = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChecklistItemsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCCount = table.Column<int>(type: "int", nullable: true),
                    ObservationCount = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditChecklists", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_DepartmentMasters_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "DepartmentMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_EmployeeMasters_AuditeeId",
                        column: x => x.AuditeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_EmployeeMasters_AuditorId",
                        column: x => x.AuditorId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablAuditChecklists_NablAuditPlans_AuditPlanId",
                        column: x => x.AuditPlanId,
                        principalTable: "NablAuditPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablAuditSummaries",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditPlanId = table.Column<long>(type: "bigint", nullable: true),
                    AuditDateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditDateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAudits = table.Column<int>(type: "int", nullable: true),
                    TotalNCs = table.Column<int>(type: "int", nullable: true),
                    MajorNCs = table.Column<int>(type: "int", nullable: true),
                    MinorNCs = table.Column<int>(type: "int", nullable: true),
                    Observations = table.Column<int>(type: "int", nullable: true),
                    FindingsSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositiveFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosureStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextAuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SummaryBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablAuditSummaries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablAuditSummaries_NablAuditPlans_AuditPlanId",
                        column: x => x.AuditPlanId,
                        principalTable: "NablAuditPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablMeetingMinutes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendaId = table.Column<long>(type: "bigint", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChairpersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttendeesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinutesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextMeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMeetingAgenda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionClosureStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablMeetingMinutes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablMeetingMinutes_NablMeetingAgendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "NablMeetingAgendas",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTrainingAttendances",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingPlanId = table.Column<long>(type: "bigint", nullable: true),
                    TrainingTopic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VenueMode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttendeesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAttendees = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTrainingAttendances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTrainingAttendances_NablTrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "NablTrainingPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NablTrainingEffectiveness",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingPlanId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrainingTopic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluationMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KnowledgeScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    SkillScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EffectivenessResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionRequired = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReEvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RevNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreparedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreparedById = table.Column<long>(type: "bigint", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewFrequencyMonths = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    ObsoleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NablTrainingEffectiveness", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NablTrainingEffectiveness_EmployeeMasters_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMasters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NablTrainingEffectiveness_NablTrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "NablTrainingPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FinancialYearChangeLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialYearId = table.Column<long>(type: "bigint", nullable: false),
                    OldYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NewYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedById = table.Column<long>(type: "bigint", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialYearChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialYearChangeLogs_FinancialYears_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "FinancialYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialYearChangeLogs_UserMasters_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "UserMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBranches",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    BranchID = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanExecute = table.Column<bool>(type: "bit", nullable: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranches", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserBranches_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBranches_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    PermissionID = table.Column<long>(type: "bigint", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserPermissions_PermissionMasters_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "PermissionMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_UserMasters_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMasters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_AdjustedToInvoiceID",
                table: "AdvancePaymentVouchers",
                column: "AdjustedToInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_CustomerID",
                table: "AdvancePaymentVouchers",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_FinancialYearId",
                table: "AdvancePaymentVouchers",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePaymentVouchers_InwardID",
                table: "AdvancePaymentVouchers",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_AmendmentRequests_ReportHeaderID",
                table: "AmendmentRequests",
                column: "ReportHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTechniqueMaster_Code",
                table: "AnalysisTechniqueMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AreaMasters_CityID",
                table: "AreaMasters",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_BankMasters_BranchID",
                table: "BankMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_BranchDisciplines_BranchID_DisciplineID",
                table: "BranchDisciplines",
                columns: new[] { "BranchID", "DisciplineID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchDisciplines_DisciplineID",
                table: "BranchDisciplines",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_OrganizationID_Code",
                table: "Branches",
                columns: new[] { "OrganizationID", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_InwardID",
                table: "ChargeEvents",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_ProformaInvoiceID",
                table: "ChargeEvents",
                column: "ProformaInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_SampleID",
                table: "ChargeEvents",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeEvents_TaxInvoiceID",
                table: "ChargeEvents",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElements_ChemicalTestID",
                table: "ChemicalTestElements",
                column: "ChemicalTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElements_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestElements",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElements_ParameterID",
                table: "ChemicalTestElements",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElements_SpecificationLineID",
                table: "ChemicalTestElements",
                column: "SpecificationLineID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestMethods_ChemicalTestID",
                table: "ChemicalTestMethods",
                column: "ChemicalTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestMethods_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestMethods",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestMethods_TestMethodSpecificationID",
                table: "ChemicalTestMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTests_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTests",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTests_SampleTestPlanID",
                table: "ChemicalTests",
                column: "SampleTestPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestTypes_ChemicalTestID",
                table: "ChemicalTestTypes",
                column: "ChemicalTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestTypes_LaboratoryTestAnalysisTypeID",
                table: "ChemicalTestTypes",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestTypes_LaboratoryTestID",
                table: "ChemicalTestTypes",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_CityMasters_StateID",
                table: "CityMasters",
                column: "StateID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationMasters_CompanyCode_Code",
                table: "ClassificationMasters",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

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
                name: "IX_ConditionMasters_Code_CompanyCode",
                table: "ConditionMasters",
                columns: new[] { "Code", "CompanyCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConditionMasters_ParameterUnitID",
                table: "ConditionMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_CustomerID",
                table: "ContactPersons",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_CustomerID",
                table: "CreditNotes",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_FinancialYearId",
                table: "CreditNotes",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNotes_TaxInvoiceID",
                table: "CreditNotes",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAmendments_ReportID",
                table: "CustomerAmendments",
                column: "ReportID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAmendments_TokenID",
                table: "CustomerAmendments",
                column: "TokenID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerChangeRequests_CustomerID",
                table: "CustomerChangeRequests",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCompanyCategories_CompanyCategoryID",
                table: "CustomerCompanyCategories",
                column: "CompanyCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCompanyCategories_CustomerID",
                table: "CustomerCompanyCategories",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDispatchModes_CustomerID",
                table: "CustomerDispatchModes",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDispatchModes_DispatchModeID",
                table: "CustomerDispatchModes",
                column: "DispatchModeID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_CustomerId",
                table: "CustomerLedgers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_InvoiceId",
                table: "CustomerLedgers",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_InwardId",
                table: "CustomerLedgers",
                column: "InwardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchaseOrders_CustomerId",
                table: "CustomerPurchaseOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeDetails_CuttingChargeSampleID",
                table: "CuttingChargeDetails",
                column: "CuttingChargeSampleID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeDetails_CuttingID",
                table: "CuttingChargeDetails",
                column: "CuttingID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeHeaders_InwardID",
                table: "CuttingChargeHeaders",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeSamples_CuttingChargeHeaderID",
                table: "CuttingChargeSamples",
                column: "CuttingChargeHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeSamples_MetalClassificationID",
                table: "CuttingChargeSamples",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingChargeSamples_SpecimenTypeId",
                table: "CuttingChargeSamples",
                column: "SpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingPriceMasters_SpecimenTypeId",
                table: "CuttingPriceMasters",
                column: "SpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingPriceVersions_CuttingPriceMasterID_EffectiveFrom",
                table: "CuttingPriceVersions",
                columns: new[] { "CuttingPriceMasterID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingPriceVersions_FinancialYearId",
                table: "CuttingPriceVersions",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_CustomerID",
                table: "DebitNotes",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_FinancialYearId",
                table: "DebitNotes",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitNotes_TaxInvoiceID",
                table: "DebitNotes",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMasters_BranchID_Code_Filtered",
                table: "DepartmentMasters",
                columns: new[] { "BranchID", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMasters_DisciplineID",
                table: "DepartmentMasters",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_DesignationMasters_RoleID",
                table: "DesignationMasters",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMaster_Code",
                table: "DimensionalFactorMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMasters_DefaultTestMethodID",
                table: "DimensionalFactorMasters",
                column: "DefaultTestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorMasters_ParameterUnitID",
                table: "DimensionalFactorMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionalFactorProductForms_ProductFormID",
                table: "DimensionalFactorProductForms",
                column: "ProductFormID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeID",
                table: "EmployeeDocuments",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_UploadReferenceID",
                table: "EmployeeDocuments",
                column: "UploadReferenceID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_BranchID",
                table: "EmployeeMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_DepartmentID",
                table: "EmployeeMasters",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_DesignationID",
                table: "EmployeeMasters",
                column: "DesignationID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_ReportingManagerID",
                table: "EmployeeMasters",
                column: "ReportingManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMasters_UserID",
                table: "EmployeeMasters",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeQualifications_EmployeeID",
                table: "EmployeeQualifications",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentDailyRecords_EnvironmentMonitoringID",
                table: "EnvironmentDailyRecords",
                column: "EnvironmentMonitoringID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAnalysisTechniques_AnalysisTechniqueID",
                table: "EquipmentAnalysisTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAnalysisTechniques_EquipmentID",
                table: "EquipmentAnalysisTechniques",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCalibration_EquipmentMasterID",
                table: "EquipmentCalibration",
                column: "EquipmentMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintenance_EquipmentMasterID",
                table: "EquipmentMaintenance",
                column: "EquipmentMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_BranchID",
                table: "EquipmentMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_EquipmentTypeID",
                table: "EquipmentMasters",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMasters_LabRoomID",
                table: "EquipmentMasters",
                column: "LabRoomID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReferenceMaterials_EquipmentMasterID",
                table: "EquipmentReferenceMaterials",
                column: "EquipmentMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSOP_EquipmentMasterID",
                table: "EquipmentSOP",
                column: "EquipmentMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialYearChangeLogs_ChangedById",
                table: "FinancialYearChangeLogs",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialYearChangeLogs_FinancialYearId",
                table: "FinancialYearChangeLogs",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTestMethods_GeneralTestID",
                table: "GeneralTestMethods",
                column: "GeneralTestID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTests_LaboratoryTestSubGroupID",
                table: "GeneralTests",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTests_SampleTestPlanID",
                table: "GeneralTests",
                column: "SampleTestPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_ReportFormatID",
                table: "GeneratedReports",
                column: "ReportFormatID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_SampleID",
                table: "GeneratedReports",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMasters_DisciplineID",
                table: "GroupMasters",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMaster_Code",
                table: "HeatTreatmentMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMasters_CoolingMediumID",
                table: "HeatTreatmentMasters",
                column: "CoolingMediumID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMasters_HeatTreatmentCategoryID",
                table: "HeatTreatmentMasters",
                column: "HeatTreatmentCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_HeatTreatmentMetalClassifications_MetalClassificationID",
                table: "HeatTreatmentMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseAliasNames_InvoiceConfigurationID",
                table: "InvoiceCaseAliasNames",
                column: "InvoiceConfigurationID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_Name_SelectionType",
                table: "InvoiceCaseConfigurations",
                columns: new[] { "Name", "SelectionType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCaseConfigurations_PriceDimensionTypeId",
                table: "InvoiceCaseConfigurations",
                column: "PriceDimensionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCasePrices_InvoiceCaseConfigID",
                table: "InvoiceCasePrices",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCasePrices_InvoiceCaseID",
                table: "InvoiceCasePrices",
                column: "InvoiceCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_AnalysisTypeID",
                table: "InvoiceCases",
                column: "AnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_FinancialYearId",
                table: "InvoiceCases",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCases_LaboratoryTestID_EffectiveFrom",
                table: "InvoiceCases",
                columns: new[] { "LaboratoryTestID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_ProformaInvoiceHeaderID",
                table: "InvoiceLineItems",
                column: "ProformaInvoiceHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_SampleInwardID",
                table: "InvoiceLineItems",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_TaxInvoiceID",
                table: "InvoiceLineItems",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardAddresses_CustomerID",
                table: "InwardAddresses",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardAddresses_InwardID",
                table: "InwardAddresses",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardContacts_InwardID",
                table: "InwardContacts",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardDispatchModes_InwardID",
                table: "InwardDispatchModes",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeEquipments_EquipmentID",
                table: "LaboratoryTestAnalysisTypeEquipments",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeEquipments_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeEquipments",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeInvoiceCases_InvoiceCaseConfigID",
                table: "LaboratoryTestAnalysisTypeInvoiceCases",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeInvoiceCases_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeInvoiceCases",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestAnalysisTypeMethods",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeParameters_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeParameters",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeParameters_ParameterID",
                table: "LaboratoryTestAnalysisTypeParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypes_LaboratoryTestSubGroupID",
                table: "LaboratoryTestAnalysisTypes",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypes_MetalClassificationID",
                table: "LaboratoryTestAnalysisTypes",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_ProductMasterID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_SpecificationGradeID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeSpecifications_SpecificationHeaderID",
                table: "LaboratoryTestAnalysisTypeSpecifications",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeTechniques_AnalysisTechniqueID",
                table: "LaboratoryTestAnalysisTypeTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestAnalysisTypeTechniques_LaboratoryTestAnalysisTypeID",
                table: "LaboratoryTestAnalysisTypeTechniques",
                column: "LaboratoryTestAnalysisTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestConditions_ConditionMasterID",
                table: "LaboratoryTestConditions",
                column: "ConditionMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestConditions_LaboratoryTestID_ConditionMasterID",
                table: "LaboratoryTestConditions",
                columns: new[] { "LaboratoryTestID", "ConditionMasterID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_LaboratoryTestID_TestMethodSpecificationID_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods",
                columns: new[] { "LaboratoryTestID", "TestMethodSpecificationID", "TestMethodSpecificationVersionID" },
                unique: true,
                filter: "[TestMethodSpecificationVersionID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_TestMethodSpecificationID",
                table: "LaboratoryTestMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestMethods",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestParameters_LaboratoryTestID_ParameterID",
                table: "LaboratoryTestParameters",
                columns: new[] { "LaboratoryTestID", "ParameterID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestParameters_ParameterID",
                table: "LaboratoryTestParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_CompanyCode_Code",
                table: "LaboratoryTests",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_DisciplineID",
                table: "LaboratoryTests",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTests_LabDepartmentID",
                table: "LaboratoryTests",
                column: "LabDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupEquipments_EquipmentID",
                table: "LaboratoryTestSubGroupEquipments",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupEquipments_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupEquipments",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupInvoiceCases_InvoiceCaseConfigID",
                table: "LaboratoryTestSubGroupInvoiceCases",
                column: "InvoiceCaseConfigID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupInvoiceCases_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupInvoiceCases",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupMethods_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupMethods",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupMethods_TestMethodSpecificationID",
                table: "LaboratoryTestSubGroupMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupMethods_TestMethodSpecificationVersionID",
                table: "LaboratoryTestSubGroupMethods",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupParameters_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupParameters",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupParameters_ParameterID",
                table: "LaboratoryTestSubGroupParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroups_LaboratoryTestID",
                table: "LaboratoryTestSubGroups",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroups_MetalClassificationID",
                table: "LaboratoryTestSubGroups",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_LaboratoryTestSubGroupID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "LaboratoryTestSubGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_ProductMasterID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_SpecificationGradeID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryTestSubGroupSpecifications_SpecificationHeaderID",
                table: "LaboratoryTestSubGroupSpecifications",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_LabRooms_BranchID",
                table: "LabRooms",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_LabRooms_DepartmentID",
                table: "LabRooms",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeMasters_BranchID",
                table: "LabScopeMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeMasters_LaboratoryTestID",
                table: "LabScopeMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecificationParameterEquipments_LabScopeSpecificationParameterID",
                table: "LabScopeSpecificationParameterEquipments",
                column: "LabScopeSpecificationParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecificationParameters_LabScopeSpecificationID",
                table: "LabScopeSpecificationParameters",
                column: "LabScopeSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecifications_LabScopeID",
                table: "LabScopeSpecifications",
                column: "LabScopeID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecifications_TestMethodSpecificationID",
                table: "LabScopeSpecifications",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_LabScopeSpecifications_TestMethodSpecificationVersionID",
                table: "LabScopeSpecifications",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_LongTermRecords_LongTermTestID",
                table: "LongTermRecords",
                column: "LongTermTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LongTermTests_SampleID",
                table: "LongTermTests",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_LongTermTests_TestResultHeaderID",
                table: "LongTermTests",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeItems_MachiningChargeMasterID",
                table: "MachiningChargeItems",
                column: "MachiningChargeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeItems_SampleID",
                table: "MachiningChargeItems",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeVersions_FinancialYearId",
                table: "MachiningChargeVersions",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MachiningChargeVersions_MachiningChargeMasterID_EffectiveFrom",
                table: "MachiningChargeVersions",
                columns: new[] { "MachiningChargeMasterID", "EffectiveFrom" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MenuMasters_ParentID",
                table: "MenuMasters",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplate_TemplateKey_Channel_Version",
                table: "MessageTemplates",
                columns: new[] { "TemplateKey", "Type", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationAnalysisTechniques_AnalysisTechniqueID",
                table: "MetalClassificationAnalysisTechniques",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationAnalysisTechniques_MetalClassificationID",
                table: "MetalClassificationAnalysisTechniques",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationMaster_Code",
                table: "MetalClassificationMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationMasters_ParentID",
                table: "MetalClassificationMasters",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_MetalClassificationParameters_ParameterID",
                table: "MetalClassificationParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_NablAccreditations_BranchID",
                table: "NablAccreditations",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_NablAccreditations_OrganizationId",
                table: "NablAccreditations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_AuditeeId",
                table: "NablAuditChecklists",
                column: "AuditeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_AuditorId",
                table: "NablAuditChecklists",
                column: "AuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_AuditPlanId",
                table: "NablAuditChecklists",
                column: "AuditPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditChecklists_DepartmentId",
                table: "NablAuditChecklists",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditPlans_LeadAuditorId",
                table: "NablAuditPlans",
                column: "LeadAuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_NablAuditSummaries_AuditPlanId",
                table: "NablAuditSummaries",
                column: "AuditPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCalibrationReviews_EquipmentId",
                table: "NablCalibrationReviews",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCompetenceRequirements_PositionId",
                table: "NablCompetenceRequirements",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_NablComplaints_CustomerId",
                table: "NablComplaints",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCrmConsumptions_ReferenceMaterialId",
                table: "NablCrmConsumptions",
                column: "ReferenceMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_NablCustomerFeedbacks_CustomerId",
                table: "NablCustomerFeedbacks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeAuthorizations_DepartmentId",
                table: "NablEmployeeAuthorizations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeAuthorizations_EmployeeId",
                table: "NablEmployeeAuthorizations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeeCompetences_EmployeeId",
                table: "NablEmployeeCompetences",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEmployeePerformanceRecords_EmployeeId",
                table: "NablEmployeePerformanceRecords",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEnvironmentMonitorings_DepartmentId",
                table: "NablEnvironmentMonitorings",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEnvironmentMonitorings_LabRoomId",
                table: "NablEnvironmentMonitorings",
                column: "LabRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_NablEquipmentHistories_EquipmentId",
                table: "NablEquipmentHistories",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablFeedbackAnalyses_CustomerFeedbackId",
                table: "NablFeedbackAnalyses",
                column: "CustomerFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_NablIncomingMaterials_SupplierId",
                table: "NablIncomingMaterials",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NablInductionTrainings_EmployeeId",
                table: "NablInductionTrainings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablIntermediateChecks_EquipmentId",
                table: "NablIntermediateChecks",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablInternalAuditors_EmployeeId",
                table: "NablInternalAuditors",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablJobDescriptions_DepartmentId",
                table: "NablJobDescriptions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablJobDescriptions_DesignationId",
                table: "NablJobDescriptions",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablMeetingAgendas_ChairpersonId",
                table: "NablMeetingAgendas",
                column: "ChairpersonId");

            migrationBuilder.CreateIndex(
                name: "IX_NablMeetingMinutes_AgendaId",
                table: "NablMeetingMinutes",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_NablNcCorrectiveActions_NCId",
                table: "NablNcCorrectiveActions",
                column: "NCId");

            migrationBuilder.CreateIndex(
                name: "IX_NablProductInspections_SupplierId",
                table: "NablProductInspections",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseIndents_DepartmentId",
                table: "NablPurchaseIndents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseMaterialVerifications_PurchaseOrderId",
                table: "NablPurchaseMaterialVerifications",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseOrders_PurchaseIndentId",
                table: "NablPurchaseOrders",
                column: "PurchaseIndentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablPurchaseOrders_SupplierId",
                table: "NablPurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NablQualityControlPlans_DepartmentId",
                table: "NablQualityControlPlans",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablResponsibilityAuthorities_DesignationId",
                table: "NablResponsibilityAuthorities",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablSkillMatrices_DesignationId",
                table: "NablSkillMatrices",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablSkillMatrixDecisions_DesignationId",
                table: "NablSkillMatrixDecisions",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTechnicalRawDatas_EquipmentId",
                table: "NablTechnicalRawDatas",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTestMethods_TestMethodStandardId",
                table: "NablTestMethods",
                column: "TestMethodStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTestRequests_CustomerId",
                table: "NablTestRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingAttendances_TrainingPlanId",
                table: "NablTrainingAttendances",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingEffectiveness_EmployeeId",
                table: "NablTrainingEffectiveness",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingEffectiveness_TrainingPlanId",
                table: "NablTrainingEffectiveness",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_NablTrainingPlans_EmployeeId",
                table: "NablTrainingPlans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberingConfigs_BranchId",
                table: "NumberingConfigs",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberingConfigs_OrganizationId",
                table: "NumberingConfigs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterDropdownOptions_ParameterID",
                table: "ParameterDropdownOptions",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterUnitEquivalentID",
                table: "ParameterMasters",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterObservationResults_ParameterMasterID",
                table: "ParameterObservationResults",
                column: "ParameterMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterObservationResults_TestObservationID",
                table: "ParameterObservationResults",
                column: "TestObservationID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterSpecimenOrientation_SpecimenOrientationID",
                table: "ParameterSpecimenOrientation",
                column: "SpecimenOrientationID");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterUnitEquivalents_BaseParameterUnitID",
                table: "ParameterUnitEquivalents",
                column: "BaseParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_CustomerId",
                table: "PaymentOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_TaxInvoiceID",
                table: "PaymentOrders",
                column: "TaxInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_CustomerId",
                table: "PaymentReceipts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_ReceiptNo",
                table: "PaymentReceipts",
                column: "ReceiptNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionMasters_MenuID",
                table: "PermissionMasters",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanHistories_PlanId",
                table: "PlanHistories",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionMaster_Code",
                table: "ProductConditionMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionMasters_LinkedHeatTreatmentID",
                table: "ProductConditionMasters",
                column: "LinkedHeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionMasters_ProductConditionCategoryID",
                table: "ProductConditionMasters",
                column: "ProductConditionCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConditionPropertyTypes_PropertyTypeID",
                table: "ProductConditionPropertyTypes",
                column: "PropertyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterMetalClassifications_MetalClassificationID",
                table: "ProductMasterMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasters_ProductSizeMasterID",
                table: "ProductMasters",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_HeatTreatmentID",
                table: "ProductMasterVersionGradeConditions",
                column: "HeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductConditionID1",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductConditionID1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductConditionID2",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductConditionID2");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductMasterVersionGradeID",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductMasterVersionGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGradeConditions_ProductSizeMasterID",
                table: "ProductMasterVersionGradeConditions",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGrades_ProductMasterVersionID",
                table: "ProductMasterVersionGrades",
                column: "ProductMasterVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersionGrades_SpecificationGradeID",
                table: "ProductMasterVersionGrades",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersions_ProductMasterID",
                table: "ProductMasterVersions",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMasterVersions_StandardOrganizationID",
                table: "ProductMasterVersions",
                column: "StandardOrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeMasters_ParameterUnitID",
                table: "ProductSizeMasters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetails_ProformaInvoiceHeaderID",
                table: "ProformaInvoiceDetails",
                column: "ProformaInvoiceHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeader_FinancialYearId",
                table: "ProformaInvoiceHeader",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeader_InwardID",
                table: "ProformaInvoiceHeader",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_CustomerPurchaseOrderID",
                table: "PurchaseOrderItems",
                column: "CustomerPurchaseOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_QuotationID",
                table: "QuotationItems",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ConvertedToInwardID",
                table: "Quotations",
                column: "ConvertedToInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CustomerID",
                table: "Quotations",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_ReplanRequests_PlanId",
                table: "ReplanRequests",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAmendmentTokens_ReportID",
                table: "ReportAmendmentTokens",
                column: "ReportID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportBlocks_ReportID",
                table: "ReportBlocks",
                column: "ReportID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatMappings_LaboratoryTestID",
                table: "ReportFormatMappings",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatMappings_ReportFormatID",
                table: "ReportFormatMappings",
                column: "ReportFormatID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatMappings_TestMethodID",
                table: "ReportFormatMappings",
                column: "TestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormat_FormatCode",
                table: "ReportFormats",
                column: "FormatCode",
                unique: true,
                filter: "[IsActive] = 1 AND [FormatCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReportFormatSections_ReportFormatID",
                table: "ReportFormatSections",
                column: "ReportFormatID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportHeaders_SampleID",
                table: "ReportHeaders",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportHeaderID",
                table: "Reports",
                column: "ReportHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplateBlocks_ReportTemplateID",
                table: "ReportTemplateBlocks",
                column: "ReportTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_TestTypeID",
                table: "ReportTemplates",
                column: "TestTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuMappings_MenuID",
                table: "RoleMenuMappings",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuMappings_RoleID",
                table: "RoleMenuMappings",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_Role_Permission",
                table: "RolePermissions",
                columns: new[] { "RoleID", "PermissionID" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionID",
                table: "RolePermissions",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleAdditionalDetails_SampleID",
                table: "SampleAdditionalDetails",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_AssignedGradeID",
                table: "SampleDetails",
                column: "AssignedGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_DisciplineID",
                table: "SampleDetails",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_InwardID",
                table: "SampleDetails",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_MetalClassificationID",
                table: "SampleDetails",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductConditionID",
                table: "SampleDetails",
                column: "ProductConditionID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductFormID",
                table: "SampleDetails",
                column: "ProductFormID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductMasterID",
                table: "SampleDetails",
                column: "ProductMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_ProductSizeMasterID",
                table: "SampleDetails",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SampleNo",
                table: "SampleDetails",
                column: "SampleNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SpecificationGradeID",
                table: "SampleDetails",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SpecimenOrientationID",
                table: "SampleDetails",
                column: "SpecimenOrientationID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_BranchID",
                table: "SampleInwards",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_CaseNo",
                table: "SampleInwards",
                column: "CaseNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_CustomerID",
                table: "SampleInwards",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_FinancialYearId",
                table: "SampleInwards",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleInwards_PurchaseOrderId",
                table: "SampleInwards",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationMasters_LaboratoryTestID",
                table: "SamplePreparationMasters",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparationMasters_TestMethodStandardID",
                table: "SamplePreparationMasters",
                column: "TestMethodStandardID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparations_AssignedToEmployeeID",
                table: "SamplePreparations",
                column: "AssignedToEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparations_EquipmentID",
                table: "SamplePreparations",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparations_InwardID",
                table: "SamplePreparations",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparations_PreparedByEmployeeID",
                table: "SamplePreparations",
                column: "PreparedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparations_SampleID",
                table: "SamplePreparations",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePreparations_VerifiedByEmployeeID",
                table: "SamplePreparations",
                column: "VerifiedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_MetalClassificationID",
                table: "SpecificationGrades",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_SpecificationHeaderID",
                table: "SpecificationGrades",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_ParameterID",
                table: "SpecificationHeaderParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_ParameterUnitEquivalentID",
                table: "SpecificationHeaderParameters",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_ParameterUnitID",
                table: "SpecificationHeaderParameters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaderParameters_SpecificationHeaderID",
                table: "SpecificationHeaderParameters",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaders_CompanyCode_Code",
                table: "SpecificationHeaders",
                columns: new[] { "CompanyCode", "Code" },
                unique: true,
                filter: "([Code] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationHeaders_StandardOrganizationID",
                table: "SpecificationHeaders",
                column: "StandardOrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineConditions_ConditionMasterID",
                table: "SpecificationLineConditions",
                column: "ConditionMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineConditions_SpecificationLineID",
                table: "SpecificationLineConditions",
                column: "SpecificationLineID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_DimensionalFactorID",
                table: "SpecificationLines",
                column: "DimensionalFactorID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_HeatTreatmentID",
                table: "SpecificationLines",
                column: "HeatTreatmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_LaboratoryTestID",
                table: "SpecificationLines",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ParameterID",
                table: "SpecificationLines",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ParameterUnitEquivalentID",
                table: "SpecificationLines",
                column: "ParameterUnitEquivalentID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ParameterUnitID",
                table: "SpecificationLines",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_ProductSizeMasterID",
                table: "SpecificationLines",
                column: "ProductSizeMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_SpecificationGradeID_SpecificationVersionID",
                table: "SpecificationLines",
                columns: new[] { "SpecificationGradeID", "SpecificationVersionID" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_SpecificationVersionID",
                table: "SpecificationLines",
                column: "SpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLines_SpecimenOrientationID",
                table: "SpecificationLines",
                column: "SpecimenOrientationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineTestMethods_LaboratoryTestID",
                table: "SpecificationLineTestMethods",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineTestMethods_SpecificationLineID",
                table: "SpecificationLineTestMethods",
                column: "SpecificationLineID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationLineTestMethods_TestMethodSpecificationID",
                table: "SpecificationLineTestMethods",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationVersionParameters_ParameterID",
                table: "SpecificationVersionParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationVersionParameters_SpecificationVersionID_ParameterID",
                table: "SpecificationVersionParameters",
                columns: new[] { "SpecificationVersionID", "ParameterID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationVersions_SpecificationHeaderID_IsDefault",
                table: "SpecificationVersions",
                columns: new[] { "SpecificationHeaderID", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationVersions_SpecificationHeaderID_Version",
                table: "SpecificationVersions",
                columns: new[] { "SpecificationHeaderID", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationMaster_Code",
                table: "SpecimenOrientationMasters",
                column: "Code",
                unique: true,
                filter: "[IsActive] = 1 AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationMasters_SpecimenOrientationCategoryID",
                table: "SpecimenOrientationMasters",
                column: "SpecimenOrientationCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationMetalClassifications_MetalClassificationID",
                table: "SpecimenOrientationMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_SpecimenOrientationProductForms_ProductFormID",
                table: "SpecimenOrientationProductForms",
                column: "ProductFormID");

            migrationBuilder.CreateIndex(
                name: "IX_StateMasters_CountryID",
                table: "StateMasters",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_SubGroupMasters_GroupID",
                table: "SubGroupMasters",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_CustomerID",
                table: "TaxInvoices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_FinancialYearId",
                table: "TaxInvoices",
                column: "FinancialYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_InwardID",
                table: "TaxInvoices",
                column: "InwardID");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInvoices_PurchaseOrderId",
                table: "TaxInvoices",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_BranchID",
                table: "TestExecutions",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_ExecutionConfigSnapshotID",
                table: "TestExecutions",
                column: "ExecutionConfigSnapshotID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_OrganizationID",
                table: "TestExecutions",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_PreviousExecutionID",
                table: "TestExecutions",
                column: "PreviousExecutionID");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecutions_UniversalTestGroupID",
                table: "TestExecutions",
                column: "UniversalTestGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupMappings_TestGroupID",
                table: "TestGroupMappings",
                column: "TestGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroupMappings_TestMethodID",
                table: "TestGroupMappings",
                column: "TestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationMetalClassifications_MetalClassificationID",
                table: "TestMethodSpecificationMetalClassifications",
                column: "MetalClassificationID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationParameters_ParameterID",
                table: "TestMethodSpecificationParameters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationParameters_ParameterUnitID",
                table: "TestMethodSpecificationParameters",
                column: "ParameterUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationParameters_TestMethodSpecificationVersionID",
                table: "TestMethodSpecificationParameters",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecifications_AnalysisTechniqueID",
                table: "TestMethodSpecifications",
                column: "AnalysisTechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSpecificationVersions_TestMethodSpecificationID_IsDefault",
                table: "TestMethodSpecificationVersions",
                columns: new[] { "TestMethodSpecificationID", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_TestMethodSubGroups_TestMethodID",
                table: "TestMethodSubGroups",
                column: "TestMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_TestObservations_TestSpecimenID",
                table: "TestObservations",
                column: "TestSpecimenID");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlans_SampleID",
                table: "TestPlans",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_LaboratoryTestID",
                table: "TestResultHeaders",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultHeaders_SampleID_LaboratoryTestID_TestPlanID",
                table: "TestResultHeaders",
                columns: new[] { "SampleID", "LaboratoryTestID", "TestPlanID" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultImages_TestResultHeaderID",
                table: "TestResultImages",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultParameters_TestResultHeaderID",
                table: "TestResultParameters",
                column: "TestResultHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TestSpecimens_TestExecutionID",
                table: "TestSpecimens",
                column: "TestExecutionID");

            migrationBuilder.CreateIndex(
                name: "IX_TestUsageStats_LaboratoryTestID_MetalClassificationID_ProductConditionID_CustomerID",
                table: "TestUsageStats",
                columns: new[] { "LaboratoryTestID", "MetalClassificationID", "ProductConditionID", "CustomerID" });

            migrationBuilder.CreateIndex(
                name: "IX_ToleranceMasters_ParameterID",
                table: "ToleranceMasters",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ToleranceMasters_SpecificationHeaderID",
                table: "ToleranceMasters",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_TpiInspections_SampleDetailID",
                table: "TpiInspections",
                column: "SampleDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_TpiInspections_SampleInwardID",
                table: "TpiInspections",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_TpiInspections_TPIMasterID",
                table: "TpiInspections",
                column: "TPIMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_BranchID",
                table: "UniversalTestGroups",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_LaboratoryTestID",
                table: "UniversalTestGroups",
                column: "LaboratoryTestID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_OrganizationID",
                table: "UniversalTestGroups",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SampleTestPlanID",
                table: "UniversalTestGroups",
                column: "SampleTestPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SpecificationGradeID",
                table: "UniversalTestGroups",
                column: "SpecificationGradeID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SpecificationHeaderID",
                table: "UniversalTestGroups",
                column: "SpecificationHeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_SpecificationVersionID",
                table: "UniversalTestGroups",
                column: "SpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_TestMethodSpecificationID",
                table: "UniversalTestGroups",
                column: "TestMethodSpecificationID");

            migrationBuilder.CreateIndex(
                name: "IX_UniversalTestGroups_TestMethodSpecificationVersionID",
                table: "UniversalTestGroups",
                column: "TestMethodSpecificationVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_BranchID",
                table: "UserBranches",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_UserID_BranchID",
                table: "UserBranches",
                columns: new[] { "UserID", "BranchID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_UserID_IsDefault",
                table: "UserBranches",
                column: "UserID",
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_UserMasters_BranchID",
                table: "UserMasters",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMasters_EmployeeID",
                table: "UserMasters",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMasters_RoleID",
                table: "UserMasters",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionID",
                table: "UserPermissions",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserID",
                table: "UserPermissions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_EmployeeID",
                table: "WorkflowActionLogs",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_StepID",
                table: "WorkflowActionLogs",
                column: "StepID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActionLogs_WorkflowID",
                table: "WorkflowActionLogs",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_CurrentStepID",
                table: "WorkflowInstances",
                column: "CurrentStepID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowID",
                table: "WorkflowInstances",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowID",
                table: "WorkflowSteps",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_StepID",
                table: "WorkflowTransitions",
                column: "StepID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocuments_EmployeeMasters_EmployeeID",
                table: "EmployeeDocuments",
                column: "EmployeeID",
                principalTable: "EmployeeMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMasters_UserMasters_UserID",
                table: "EmployeeMasters",
                column: "UserID",
                principalTable: "UserMasters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMasters_Branches_BranchID",
                table: "DepartmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMasters_Branches_BranchID",
                table: "EmployeeMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_Branches_BranchID",
                table: "UserMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentMasters_DisciplineMasters_DisciplineID",
                table: "DepartmentMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignationMasters_RoleMasters_RoleID",
                table: "DesignationMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_RoleMasters_RoleID",
                table: "UserMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMasters_EmployeeMasters_EmployeeID",
                table: "UserMasters");

            migrationBuilder.DropTable(
                name: "AdvancePaymentVouchers");

            migrationBuilder.DropTable(
                name: "AmendmentRequests");

            migrationBuilder.DropTable(
                name: "AuthorizedSignatories");

            migrationBuilder.DropTable(
                name: "BankMasters");

            migrationBuilder.DropTable(
                name: "BranchDisciplines");

            migrationBuilder.DropTable(
                name: "CalibrationAgencyMasters");

            migrationBuilder.DropTable(
                name: "ChargeEvents");

            migrationBuilder.DropTable(
                name: "ChemicalTestElements");

            migrationBuilder.DropTable(
                name: "ChemicalTestMethods");

            migrationBuilder.DropTable(
                name: "ChemicalTestTypes");

            migrationBuilder.DropTable(
                name: "ClassificationMasters");

            migrationBuilder.DropTable(
                name: "CompanyMasters");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "ContactPersons");

            migrationBuilder.DropTable(
                name: "CourierMasters");

            migrationBuilder.DropTable(
                name: "CreditNotes");

            migrationBuilder.DropTable(
                name: "CustomerAmendments");

            migrationBuilder.DropTable(
                name: "CustomerChangeRequests");

            migrationBuilder.DropTable(
                name: "CustomerCompanyCategories");

            migrationBuilder.DropTable(
                name: "CustomerDispatchModes");

            migrationBuilder.DropTable(
                name: "CustomerLedgers");

            migrationBuilder.DropTable(
                name: "CuttingChargeDetails");

            migrationBuilder.DropTable(
                name: "CuttingPriceVersions");

            migrationBuilder.DropTable(
                name: "DebitNotes");

            migrationBuilder.DropTable(
                name: "DimensionalFactorProductForms");

            migrationBuilder.DropTable(
                name: "EmployeeDocuments");

            migrationBuilder.DropTable(
                name: "EmployeeQualifications");

            migrationBuilder.DropTable(
                name: "EnvironmentDailyRecords");

            migrationBuilder.DropTable(
                name: "EquipmentAnalysisTechniques");

            migrationBuilder.DropTable(
                name: "EquipmentCalibration");

            migrationBuilder.DropTable(
                name: "EquipmentMaintenance");

            migrationBuilder.DropTable(
                name: "EquipmentReferenceMaterials");

            migrationBuilder.DropTable(
                name: "EquipmentSOP");

            migrationBuilder.DropTable(
                name: "FinancialYearChangeLogs");

            migrationBuilder.DropTable(
                name: "GeneralTestMethods");

            migrationBuilder.DropTable(
                name: "GeneratedReports");

            migrationBuilder.DropTable(
                name: "GstConfigs");

            migrationBuilder.DropTable(
                name: "HardnessEquivalences");

            migrationBuilder.DropTable(
                name: "HeatTreatmentMetalClassifications");

            migrationBuilder.DropTable(
                name: "IndustryMasters");

            migrationBuilder.DropTable(
                name: "InvoiceCaseAliasNames");

            migrationBuilder.DropTable(
                name: "InvoiceCasePrices");

            migrationBuilder.DropTable(
                name: "InvoiceLineItems");

            migrationBuilder.DropTable(
                name: "InwardAddresses");

            migrationBuilder.DropTable(
                name: "InwardContacts");

            migrationBuilder.DropTable(
                name: "InwardDispatchModes");

            migrationBuilder.DropTable(
                name: "ItemMasters");

            migrationBuilder.DropTable(
                name: "JobExecutionLogs");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeEquipments");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeMethods");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeParameters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeSpecifications");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypeTechniques");

            migrationBuilder.DropTable(
                name: "LaboratoryTestConditions");

            migrationBuilder.DropTable(
                name: "LaboratoryTestMethods");

            migrationBuilder.DropTable(
                name: "LaboratoryTestParameters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupEquipments");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupInvoiceCases");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupMethods");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupParameters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroupSpecifications");

            migrationBuilder.DropTable(
                name: "LabScopeChangeLogs");

            migrationBuilder.DropTable(
                name: "LabScopeSpecificationParameterEquipments");

            migrationBuilder.DropTable(
                name: "LongTermRecords");

            migrationBuilder.DropTable(
                name: "MachineDataLogs");

            migrationBuilder.DropTable(
                name: "MachiningChargeItems");

            migrationBuilder.DropTable(
                name: "MachiningChargeVersions");

            migrationBuilder.DropTable(
                name: "MakerMasters");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "MetalClassificationAnalysisTechniques");

            migrationBuilder.DropTable(
                name: "MetalClassificationParameters");

            migrationBuilder.DropTable(
                name: "NablAccreditations");

            migrationBuilder.DropTable(
                name: "NablApprovedSuppliers");

            migrationBuilder.DropTable(
                name: "NablAuditChecklists");

            migrationBuilder.DropTable(
                name: "NablAuditLogs");

            migrationBuilder.DropTable(
                name: "NablAuditSummaries");

            migrationBuilder.DropTable(
                name: "NablCalibrationReviews");

            migrationBuilder.DropTable(
                name: "NablCompetenceRequirements");

            migrationBuilder.DropTable(
                name: "NablComplaints");

            migrationBuilder.DropTable(
                name: "NablCrmConsumptions");

            migrationBuilder.DropTable(
                name: "NablDocumentChangeRequests");

            migrationBuilder.DropTable(
                name: "NablDocumentReviews");

            migrationBuilder.DropTable(
                name: "NablEmployeeAuthorizations");

            migrationBuilder.DropTable(
                name: "NablEmployeeCompetences");

            migrationBuilder.DropTable(
                name: "NablEmployeePerformanceRecords");

            migrationBuilder.DropTable(
                name: "NablEquipmentHistories");

            migrationBuilder.DropTable(
                name: "NablFeedbackAnalyses");

            migrationBuilder.DropTable(
                name: "NablFormAttachments");

            migrationBuilder.DropTable(
                name: "NablFormRevisionHistory");

            migrationBuilder.DropTable(
                name: "NablIncomingMaterials");

            migrationBuilder.DropTable(
                name: "NablInductionTrainings");

            migrationBuilder.DropTable(
                name: "NablIntermediateChecks");

            migrationBuilder.DropTable(
                name: "NablInternalAuditors");

            migrationBuilder.DropTable(
                name: "NablJobDescriptions");

            migrationBuilder.DropTable(
                name: "NablMasterDocuments");

            migrationBuilder.DropTable(
                name: "NablMeasurementUncertainties");

            migrationBuilder.DropTable(
                name: "NablMeetingMinutes");

            migrationBuilder.DropTable(
                name: "NablMethodValidations");

            migrationBuilder.DropTable(
                name: "NablMethodVerifications");

            migrationBuilder.DropTable(
                name: "NablNcCorrectiveActions");

            migrationBuilder.DropTable(
                name: "NablProductInspections");

            migrationBuilder.DropTable(
                name: "NablPtIlcPlans");

            migrationBuilder.DropTable(
                name: "NablPurchaseMaterialVerifications");

            migrationBuilder.DropTable(
                name: "NablQualityControlPlans");

            migrationBuilder.DropTable(
                name: "NablResponsibilityAuthorities");

            migrationBuilder.DropTable(
                name: "NablRetestings");

            migrationBuilder.DropTable(
                name: "NablRiskAssessments");

            migrationBuilder.DropTable(
                name: "NablSampleInwardRegisters");

            migrationBuilder.DropTable(
                name: "NablSampleLabels");

            migrationBuilder.DropTable(
                name: "NablSampleMusterRegisters");

            migrationBuilder.DropTable(
                name: "NablSkillMatrices");

            migrationBuilder.DropTable(
                name: "NablSkillMatrixDecisions");

            migrationBuilder.DropTable(
                name: "NablSupplierConfidentialities");

            migrationBuilder.DropTable(
                name: "NablSupplierEvaluations");

            migrationBuilder.DropTable(
                name: "NablSupplierRegistrations");

            migrationBuilder.DropTable(
                name: "NablTechnicalRawDatas");

            migrationBuilder.DropTable(
                name: "NablTestMethods");

            migrationBuilder.DropTable(
                name: "NablTestReports");

            migrationBuilder.DropTable(
                name: "NablTestRequests");

            migrationBuilder.DropTable(
                name: "NablTrainingAttendances");

            migrationBuilder.DropTable(
                name: "NablTrainingEffectiveness");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NumberingConfigs");

            migrationBuilder.DropTable(
                name: "OEMMasters");

            migrationBuilder.DropTable(
                name: "OrganisationMasters");

            migrationBuilder.DropTable(
                name: "ParameterDropdownOptions");

            migrationBuilder.DropTable(
                name: "ParameterObservationResults");

            migrationBuilder.DropTable(
                name: "ParameterSpecimenOrientation");

            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "PaymentReceipts");

            migrationBuilder.DropTable(
                name: "PlanHistories");

            migrationBuilder.DropTable(
                name: "ProductConditionPropertyTypes");

            migrationBuilder.DropTable(
                name: "ProductMasterMetalClassifications");

            migrationBuilder.DropTable(
                name: "ProductMasterVersionGradeConditions");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceDetails");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "QuotationItems");

            migrationBuilder.DropTable(
                name: "RemarkMasters");

            migrationBuilder.DropTable(
                name: "ReplanRequests");

            migrationBuilder.DropTable(
                name: "ReportBlocks");

            migrationBuilder.DropTable(
                name: "ReportFormatMappings");

            migrationBuilder.DropTable(
                name: "ReportFormatSections");

            migrationBuilder.DropTable(
                name: "ReportTemplateBlocks");

            migrationBuilder.DropTable(
                name: "RoleMenuMappings");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SampleAdditionalDetails");

            migrationBuilder.DropTable(
                name: "SamplePreparationMasters");

            migrationBuilder.DropTable(
                name: "SamplePreparations");

            migrationBuilder.DropTable(
                name: "SampleStatusHistories");

            migrationBuilder.DropTable(
                name: "SiteActivities");

            migrationBuilder.DropTable(
                name: "SiteErrors");

            migrationBuilder.DropTable(
                name: "SpecificationHeaderParameters");

            migrationBuilder.DropTable(
                name: "SpecificationLineConditions");

            migrationBuilder.DropTable(
                name: "SpecificationLineTestMethods");

            migrationBuilder.DropTable(
                name: "SpecificationVersionParameters");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationMetalClassifications");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationProductForms");

            migrationBuilder.DropTable(
                name: "SubContractorMasters");

            migrationBuilder.DropTable(
                name: "SubGroupMasters");

            migrationBuilder.DropTable(
                name: "TaxMasters");

            migrationBuilder.DropTable(
                name: "TestConditionDimensions");

            migrationBuilder.DropTable(
                name: "TestGroupMappings");

            migrationBuilder.DropTable(
                name: "TestMasters");

            migrationBuilder.DropTable(
                name: "TestMethodSpecificationMetalClassifications");

            migrationBuilder.DropTable(
                name: "TestMethodSpecificationParameters");

            migrationBuilder.DropTable(
                name: "TestMethodSubGroups");

            migrationBuilder.DropTable(
                name: "TestResultImages");

            migrationBuilder.DropTable(
                name: "TestResultParameters");

            migrationBuilder.DropTable(
                name: "TestTypeMasters");

            migrationBuilder.DropTable(
                name: "TestUsageStats");

            migrationBuilder.DropTable(
                name: "ToleranceMasters");

            migrationBuilder.DropTable(
                name: "TpiInspections");

            migrationBuilder.DropTable(
                name: "UniversalCodeTypeMasters");

            migrationBuilder.DropTable(
                name: "UOMMasters");

            migrationBuilder.DropTable(
                name: "UserBranches");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "UserPushSubscriptions");

            migrationBuilder.DropTable(
                name: "VendorMasters");

            migrationBuilder.DropTable(
                name: "WorkflowActionLogs");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions");

            migrationBuilder.DropTable(
                name: "ChemicalTests");

            migrationBuilder.DropTable(
                name: "AreaMasters");

            migrationBuilder.DropTable(
                name: "CurrencyMasters");

            migrationBuilder.DropTable(
                name: "ReportAmendmentTokens");

            migrationBuilder.DropTable(
                name: "CompanyCategoryMasters");

            migrationBuilder.DropTable(
                name: "DispatchModeMasters");

            migrationBuilder.DropTable(
                name: "CuttingChargeSamples");

            migrationBuilder.DropTable(
                name: "CuttingPriceMasters");

            migrationBuilder.DropTable(
                name: "UploadFiles");

            migrationBuilder.DropTable(
                name: "NablEnvironmentMonitorings");

            migrationBuilder.DropTable(
                name: "GeneralTests");

            migrationBuilder.DropTable(
                name: "InvoiceCases");

            migrationBuilder.DropTable(
                name: "InvoiceCaseConfigurations");

            migrationBuilder.DropTable(
                name: "LabScopeSpecificationParameters");

            migrationBuilder.DropTable(
                name: "LongTermTests");

            migrationBuilder.DropTable(
                name: "MachiningChargeMasters");

            migrationBuilder.DropTable(
                name: "NablAuditPlans");

            migrationBuilder.DropTable(
                name: "NablReferenceMaterials");

            migrationBuilder.DropTable(
                name: "NablCustomerFeedbacks");

            migrationBuilder.DropTable(
                name: "NablMeetingAgendas");

            migrationBuilder.DropTable(
                name: "NablNonConformingWorks");

            migrationBuilder.DropTable(
                name: "NablPurchaseOrders");

            migrationBuilder.DropTable(
                name: "NablTrainingPlans");

            migrationBuilder.DropTable(
                name: "TestObservations");

            migrationBuilder.DropTable(
                name: "TaxInvoices");

            migrationBuilder.DropTable(
                name: "PropertyTypeMasters");

            migrationBuilder.DropTable(
                name: "ProductMasterVersionGrades");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceHeader");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "ReportFormats");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "EquipmentMasters");

            migrationBuilder.DropTable(
                name: "ConditionMasters");

            migrationBuilder.DropTable(
                name: "SpecificationLines");

            migrationBuilder.DropTable(
                name: "GroupMasters");

            migrationBuilder.DropTable(
                name: "TestGroups");

            migrationBuilder.DropTable(
                name: "TPIMasters");

            migrationBuilder.DropTable(
                name: "PermissionMasters");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "CityMasters");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "CuttingChargeHeaders");

            migrationBuilder.DropTable(
                name: "SpecimenTypeMasters");

            migrationBuilder.DropTable(
                name: "LaboratoryTestAnalysisTypes");

            migrationBuilder.DropTable(
                name: "PriceDimensionTypes");

            migrationBuilder.DropTable(
                name: "LabScopeSpecifications");

            migrationBuilder.DropTable(
                name: "TestResultHeaders");

            migrationBuilder.DropTable(
                name: "NablPurchaseIndents");

            migrationBuilder.DropTable(
                name: "SupplierMasters");

            migrationBuilder.DropTable(
                name: "TestSpecimens");

            migrationBuilder.DropTable(
                name: "ProductMasterVersions");

            migrationBuilder.DropTable(
                name: "EquipmentTypeMasters");

            migrationBuilder.DropTable(
                name: "LabRooms");

            migrationBuilder.DropTable(
                name: "DimensionalFactorMasters");

            migrationBuilder.DropTable(
                name: "ParameterMasters");

            migrationBuilder.DropTable(
                name: "MenuMasters");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropTable(
                name: "StateMasters");

            migrationBuilder.DropTable(
                name: "ReportHeaders");

            migrationBuilder.DropTable(
                name: "LaboratoryTestSubGroups");

            migrationBuilder.DropTable(
                name: "LabScopeMasters");

            migrationBuilder.DropTable(
                name: "TestExecutions");

            migrationBuilder.DropTable(
                name: "ParameterUnitEquivalents");

            migrationBuilder.DropTable(
                name: "CountryMasters");

            migrationBuilder.DropTable(
                name: "ExecutionConfigSnapshots");

            migrationBuilder.DropTable(
                name: "UniversalTestGroups");

            migrationBuilder.DropTable(
                name: "LaboratoryTests");

            migrationBuilder.DropTable(
                name: "SpecificationVersions");

            migrationBuilder.DropTable(
                name: "TestMethodSpecificationVersions");

            migrationBuilder.DropTable(
                name: "TestPlans");

            migrationBuilder.DropTable(
                name: "TestMethodSpecifications");

            migrationBuilder.DropTable(
                name: "SampleDetails");

            migrationBuilder.DropTable(
                name: "AnalysisTechniqueMasters");

            migrationBuilder.DropTable(
                name: "ProductConditionMasters");

            migrationBuilder.DropTable(
                name: "ProductFormMasters");

            migrationBuilder.DropTable(
                name: "ProductMasters");

            migrationBuilder.DropTable(
                name: "SampleInwards");

            migrationBuilder.DropTable(
                name: "SpecificationGrades");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationMasters");

            migrationBuilder.DropTable(
                name: "HeatTreatmentMasters");

            migrationBuilder.DropTable(
                name: "ProductConditionCategoryMasters");

            migrationBuilder.DropTable(
                name: "ProductSizeMasters");

            migrationBuilder.DropTable(
                name: "CustomerPurchaseOrders");

            migrationBuilder.DropTable(
                name: "FinancialYears");

            migrationBuilder.DropTable(
                name: "MetalClassificationMasters");

            migrationBuilder.DropTable(
                name: "SpecificationHeaders");

            migrationBuilder.DropTable(
                name: "SpecimenOrientationCategoryMasters");

            migrationBuilder.DropTable(
                name: "CoolingMediumMasters");

            migrationBuilder.DropTable(
                name: "HeatTreatmentCategoryMasters");

            migrationBuilder.DropTable(
                name: "ParameterUnitMasters");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "StandardOrganizationMasters");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "DisciplineMasters");

            migrationBuilder.DropTable(
                name: "RoleMasters");

            migrationBuilder.DropTable(
                name: "EmployeeMasters");

            migrationBuilder.DropTable(
                name: "DepartmentMasters");

            migrationBuilder.DropTable(
                name: "DesignationMasters");

            migrationBuilder.DropTable(
                name: "UserMasters");
        }
    }
}
