using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class PurgeLegacyMetallurgyFromSpecificationGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add Remarks column safely if not exists (preserves existing values if already present)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'Remarks' AND object_id = OBJECT_ID('SpecificationGrades'))
                BEGIN
                    ALTER TABLE SpecificationGrades ADD Remarks nvarchar(500) NULL;
                END
            ");

            // 2. Dynamically discover and drop all default constraints attached to target legacy columns
            migrationBuilder.Sql(@"
                DECLARE @dropDefSql NVARCHAR(MAX) = N'';
                SELECT @dropDefSql += N'ALTER TABLE SpecificationGrades DROP CONSTRAINT ' + QUOTENAME(dc.name) + ';'
                FROM sys.default_constraints dc
                JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
                WHERE c.object_id = OBJECT_ID('SpecificationGrades') 
                  AND c.name IN ('MetalClassificationID', 'IsUNS', 'UNSSteelNumber');
                IF @dropDefSql <> N'' EXEC sp_executesql @dropDefSql;
            ");

            // 3. Dynamically discover and drop all check constraints attached to target legacy columns
            migrationBuilder.Sql(@"
                DECLARE @dropChkSql NVARCHAR(MAX) = N'';
                SELECT @dropChkSql += N'ALTER TABLE SpecificationGrades DROP CONSTRAINT ' + QUOTENAME(cc.name) + ';'
                FROM sys.check_constraints cc
                JOIN sys.columns c ON cc.parent_object_id = c.object_id AND cc.parent_column_id = c.column_id
                WHERE c.object_id = OBJECT_ID('SpecificationGrades') 
                  AND c.name IN ('MetalClassificationID', 'IsUNS', 'UNSSteelNumber');
                IF @dropChkSql <> N'' EXEC sp_executesql @dropChkSql;
            ");

            // 4. Dynamically discover and drop all foreign keys on MetalClassificationID
            migrationBuilder.Sql(@"
                DECLARE @dropFkSql NVARCHAR(MAX) = N'';
                SELECT @dropFkSql += N'ALTER TABLE SpecificationGrades DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';'
                FROM sys.foreign_keys fk
                JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                WHERE c.object_id = OBJECT_ID('SpecificationGrades') AND c.name = 'MetalClassificationID';
                IF @dropFkSql <> N'' EXEC sp_executesql @dropFkSql;
            ");

            // 5. Dynamically discover and drop all indexes referencing MetalClassificationID
            migrationBuilder.Sql(@"
                DECLARE @dropIdxSql NVARCHAR(MAX) = N'';
                SELECT @dropIdxSql += N'DROP INDEX ' + QUOTENAME(i.name) + N' ON SpecificationGrades;'
                FROM sys.indexes i
                JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                WHERE c.object_id = OBJECT_ID('SpecificationGrades') AND c.name = 'MetalClassificationID' AND i.is_primary_key = 0;
                IF @dropIdxSql <> N'' EXEC sp_executesql @dropIdxSql;
            ");

            // 6. Drop legacy metallurgy columns safely if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'MetalClassificationID' AND object_id = OBJECT_ID('SpecificationGrades'))
                BEGIN
                    ALTER TABLE SpecificationGrades DROP COLUMN MetalClassificationID;
                END

                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'IsUNS' AND object_id = OBJECT_ID('SpecificationGrades'))
                BEGIN
                    ALTER TABLE SpecificationGrades DROP COLUMN IsUNS;
                END

                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'UNSSteelNumber' AND object_id = OBJECT_ID('SpecificationGrades'))
                BEGIN
                    ALTER TABLE SpecificationGrades DROP COLUMN UNSSteelNumber;
                END
            ");

            // 7. Normalize legacy NULL IsActive values to 1 WITHOUT modifying explicitly inactive rows
            migrationBuilder.Sql("UPDATE SpecificationGrades SET IsActive = 1 WHERE IsActive IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "SpecificationGrades");

            migrationBuilder.AddColumn<bool>(
                name: "IsUNS",
                table: "SpecificationGrades",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MetalClassificationID",
                table: "SpecificationGrades",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UNSSteelNumber",
                table: "SpecificationGrades",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationGrades_MetalClassificationID",
                table: "SpecificationGrades",
                column: "MetalClassificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationGrades_MetalClassificationMasters_MetalClassificationID",
                table: "SpecificationGrades",
                column: "MetalClassificationID",
                principalTable: "MetalClassificationMasters",
                principalColumn: "ID");
        }
    }
}
