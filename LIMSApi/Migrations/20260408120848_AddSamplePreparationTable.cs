using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSamplePreparationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SamplePreparations — conditional create (safe to run on any DB)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SamplePreparations')
                BEGIN
                    CREATE TABLE [SamplePreparations] (
                        [ID]                        bigint IDENTITY(1,1) NOT NULL,
                        [SampleID]                  bigint NOT NULL,
                        [InwardID]                  bigint NOT NULL,
                        [Status]                    nvarchar(30) NOT NULL DEFAULT 'Pending',
                        [AssignedToEmployeeID]      bigint NULL,
                        [PreparedByEmployeeID]      bigint NULL,
                        [VerifiedByEmployeeID]      bigint NULL,
                        [AssignedOn]                datetime2 NULL,
                        [StartedOn]                 datetime2 NULL,
                        [CompletedOn]               datetime2 NULL,
                        [VerifiedOn]                datetime2 NULL,
                        [EquipmentID]               bigint NULL,
                        [PreparationMethod]         nvarchar(200) NULL,
                        [Temperature]               decimal(5,1) NULL,
                        [Humidity]                  decimal(5,1) NULL,
                        [PreparationInstructions]   nvarchar(1000) NULL,
                        [PreConditionNotes]         nvarchar(1000) NULL,
                        [PostConditionNotes]        nvarchar(1000) NULL,
                        [VerificationRemarks]       nvarchar(500) NULL,
                        [CuttingChargesTotal]       decimal(18,2) NOT NULL DEFAULT 0,
                        [MachiningChargesTotal]     decimal(18,2) NOT NULL DEFAULT 0,
                        [OtherChargesTotal]         decimal(18,2) NOT NULL DEFAULT 0,
                        -- AuditProperty fields
                        [CreatedBy]                 bigint NULL,
                        [CreatedOn]                 datetime2 NULL,
                        [ModifiedBy]                bigint NULL,
                        [ModifiedOn]                datetime2 NULL,
                        [CompanyCode]               nvarchar(max) NULL,
                        [IsActive]                  bit NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_SamplePreparations] PRIMARY KEY ([ID])
                    );
                END
            ");

            // SamplePreparationMasters — conditional create
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SamplePreparationMasters')
                BEGIN
                    CREATE TABLE [SamplePreparationMasters] (
                        [ID]                    bigint IDENTITY(1,1) NOT NULL,
                        [TestMethodStandardID]  bigint NULL,
                        [LaboratoryTestID]      bigint NULL,
                        [SpecimenType]          nvarchar(200) NOT NULL,
                        [Dimensions]            nvarchar(500) NULL,
                        [MaterialType]          nvarchar(200) NULL,
                        [Charges]               decimal(18,2) NOT NULL DEFAULT 0,
                        [Remark]                nvarchar(500) NULL,
                        -- AuditProperty fields
                        [CreatedBy]             bigint NULL,
                        [CreatedOn]             datetime2 NULL,
                        [ModifiedBy]            bigint NULL,
                        [ModifiedOn]            datetime2 NULL,
                        [CompanyCode]           nvarchar(max) NULL,
                        [IsActive]              bit NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_SamplePreparationMasters] PRIMARY KEY ([ID])
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SamplePreparations')
                    DROP TABLE [SamplePreparations];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SamplePreparationMasters')
                    DROP TABLE [SamplePreparationMasters];
            ");
        }
    }
}
