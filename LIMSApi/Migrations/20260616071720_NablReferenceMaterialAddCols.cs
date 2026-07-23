using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMSApi.Migrations
{
    /// <inheritdoc />
    public partial class NablReferenceMaterialAddCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                table: "NablReferenceMaterials",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CertificationDate",
                table: "NablReferenceMaterials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialQuantity",
                table: "NablReferenceMaterials",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MaterialDescription",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrixType",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumQuantity",
                table: "NablReferenceMaterials",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ParameterJson",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageLocation",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Traceability",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "NablReferenceMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidityDate",
                table: "NablReferenceMaterials",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "CertificationDate",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "InitialQuantity",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialDescription",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "MatrixType",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "MinimumQuantity",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "ParameterJson",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "StorageLocation",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "Traceability",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "NablReferenceMaterials");

            migrationBuilder.DropColumn(
                name: "ValidityDate",
                table: "NablReferenceMaterials");

            
        }
    }
}
