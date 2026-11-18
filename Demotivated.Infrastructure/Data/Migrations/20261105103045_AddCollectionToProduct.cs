using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demotivated.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CollectionId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_CollectionId_CategoryId",
                table: "Products",
                columns: new[] { "CollectionId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CollectionCategories_CollectionId_CategoryId",
                table: "Products",
                columns: new[] { "CollectionId", "CategoryId" },
                principalTable: "CollectionCategories",
                principalColumns: new[] { "CollectionId", "CategoryId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Collections_CollectionId",
                table: "Products",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CollectionCategories_CollectionId_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Collections_CollectionId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CollectionId_CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Products");
        }
    }
}
