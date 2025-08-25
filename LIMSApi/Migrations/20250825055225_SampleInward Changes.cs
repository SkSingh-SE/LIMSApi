using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class SampleInwardChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleInwards",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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
                    RequestFilePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RequestFileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "TestPlans",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlans", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InwardAddresses",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactPersonID = table.Column<long>(type: "bigint", nullable: false),
                    ContactPersonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InwardAddresses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InwardAddresses_SampleInwards_SampleID",
                        column: x => x.SampleID,
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendBill = table.Column<bool>(type: "bit", nullable: false),
                    SendReport = table.Column<bool>(type: "bit", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InwardContacts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InwardContacts_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SampleAdditionalDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleAdditionalDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleAdditionalDetails_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SampleDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    UploadReferenceID = table.Column<long>(type: "bigint", nullable: true),
                    SampleFilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleDetails_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SampleDispatchModes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleID = table.Column<long>(type: "bigint", nullable: false),
                    DispatchModeID = table.Column<long>(type: "bigint", nullable: false),
                    SampleInwardID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDispatchModes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SampleDispatchModes_SampleInwards_SampleInwardID",
                        column: x => x.SampleInwardID,
                        principalTable: "SampleInwards",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChemicalTest",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetalClassificationID = table.Column<long>(type: "bigint", nullable: false),
                    Specification1 = table.Column<long>(type: "bigint", nullable: false),
                    Specification2 = table.Column<long>(type: "bigint", nullable: true),
                    TestMethod = table.Column<long>(type: "bigint", nullable: false),
                    SampleTestPlanID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTest_TestPlans_SampleTestPlanID",
                        column: x => x.SampleTestPlanID,
                        principalTable: "TestPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "GeneralTest",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Specification1 = table.Column<long>(type: "bigint", nullable: false),
                    Specification2 = table.Column<long>(type: "bigint", nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleTestPlanID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralTest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GeneralTest_TestPlans_SampleTestPlanID",
                        column: x => x.SampleTestPlanID,
                        principalTable: "TestPlans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChemicalTestElement",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterID = table.Column<long>(type: "bigint", nullable: false),
                    Selected = table.Column<bool>(type: "bit", nullable: false),
                    ChemicalTestID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChemicalTestElement", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChemicalTestElement_ChemicalTest_ChemicalTestID",
                        column: x => x.ChemicalTestID,
                        principalTable: "ChemicalTest",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "GeneralTestMethod",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMethodID = table.Column<long>(type: "bigint", nullable: false),
                    StandardID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UlrNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cancel = table.Column<bool>(type: "bit", nullable: false),
                    GeneralTestID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralTestMethod", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GeneralTestMethod_GeneralTest_GeneralTestID",
                        column: x => x.GeneralTestID,
                        principalTable: "GeneralTest",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTest_SampleTestPlanID",
                table: "ChemicalTest",
                column: "SampleTestPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_ChemicalTestElement_ChemicalTestID",
                table: "ChemicalTestElement",
                column: "ChemicalTestID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTest_SampleTestPlanID",
                table: "GeneralTest",
                column: "SampleTestPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTestMethod_GeneralTestID",
                table: "GeneralTestMethod",
                column: "GeneralTestID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardAddresses_SampleID",
                table: "InwardAddresses",
                column: "SampleID");

            migrationBuilder.CreateIndex(
                name: "IX_InwardContacts_SampleInwardID",
                table: "InwardContacts",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleAdditionalDetails_SampleInwardID",
                table: "SampleAdditionalDetails",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDetails_SampleInwardID",
                table: "SampleDetails",
                column: "SampleInwardID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDispatchModes_SampleInwardID",
                table: "SampleDispatchModes",
                column: "SampleInwardID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChemicalTestElement");

            migrationBuilder.DropTable(
                name: "GeneralTestMethod");

            migrationBuilder.DropTable(
                name: "InwardAddresses");

            migrationBuilder.DropTable(
                name: "InwardContacts");

            migrationBuilder.DropTable(
                name: "SampleAdditionalDetails");

            migrationBuilder.DropTable(
                name: "SampleDetails");

            migrationBuilder.DropTable(
                name: "SampleDispatchModes");

            migrationBuilder.DropTable(
                name: "ChemicalTest");

            migrationBuilder.DropTable(
                name: "GeneralTest");

            migrationBuilder.DropTable(
                name: "SampleInwards");

            migrationBuilder.DropTable(
                name: "TestPlans");
        }
    }
}
