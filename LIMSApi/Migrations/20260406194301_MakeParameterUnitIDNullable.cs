using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeParameterUnitIDNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing FK constraint if present
            migrationBuilder.Sql(@"
                DECLARE @fkName NVARCHAR(256) = (
                    SELECT TOP 1 fk.name
                    FROM sys.foreign_keys fk
                    JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
                    WHERE OBJECT_NAME(fk.parent_object_id) = 'ParameterMasters'
                    AND c.name = 'ParameterUnitID'
                );
                IF @fkName IS NOT NULL
                    EXEC('ALTER TABLE [ParameterMasters] DROP CONSTRAINT [' + @fkName + ']');
            ");

            migrationBuilder.AlterColumn<long>(
                name: "ParameterUnitID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters",
                column: "ParameterUnitID",
                principalTable: "ParameterUnitMasters",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Set null values to a default unit before making NOT NULL
            migrationBuilder.Sql("UPDATE [ParameterMasters] SET [ParameterUnitID] = 1 WHERE [ParameterUnitID] IS NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterMasters_ParameterUnitMasters_ParameterUnitID",
                table: "ParameterMasters");

            migrationBuilder.AlterColumn<long>(
                name: "ParameterUnitID",
                table: "ParameterMasters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
