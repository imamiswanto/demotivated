using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demotivated.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseInsensitiveSkuIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_SKU",
                table: "ProductVariants");

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX "IX_ProductVariants_SKU_CaseInsensitive"
                ON "ProductVariants" (LOWER("SKU"));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "IX_ProductVariants_SKU_CaseInsensitive";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_SKU",
                table: "ProductVariants",
                column: "SKU",
                unique: true);
        }
    }
}
