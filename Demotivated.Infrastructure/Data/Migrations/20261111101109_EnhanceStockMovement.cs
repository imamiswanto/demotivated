using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demotivated.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceStockMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "StockMovements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "StockMovements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockAfter",
                table: "StockMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockBefore",
                table: "StockMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_StockAfter_NonNegative",
                table: "StockMovements",
                sql: "\"StockAfter\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_StockBefore_NonNegative",
                table: "StockMovements",
                sql: "\"StockBefore\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_StockConsistency",
                table: "StockMovements",
                sql: "\"StockAfter\" = \"StockBefore\" + \"Quantity\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_StockAfter_NonNegative",
                table: "StockMovements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_StockBefore_NonNegative",
                table: "StockMovements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_StockConsistency",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "StockAfter",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "StockBefore",
                table: "StockMovements");
        }
    }
}
