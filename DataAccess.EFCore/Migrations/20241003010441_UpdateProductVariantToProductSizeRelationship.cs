using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductVariantToProductSizeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSizeProductVariant");

            migrationBuilder.AddColumn<int>(
                name: "ProductSizeId",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductSizeId",
                table: "ProductVariants",
                column: "ProductSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductSizes_ProductSizeId",
                table: "ProductVariants",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductSizes_ProductSizeId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductSizeId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "ProductVariants");

            migrationBuilder.CreateTable(
                name: "ProductSizeProductVariant",
                columns: table => new
                {
                    ProductSizesId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizeProductVariant", x => new { x.ProductSizesId, x.ProductVariantsId });
                    table.ForeignKey(
                        name: "FK_ProductSizeProductVariant_ProductSizes_ProductSizesId",
                        column: x => x.ProductSizesId,
                        principalTable: "ProductSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ProductSizeProductVariant_ProductVariants_ProductVariantsId",
                        column: x => x.ProductVariantsId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizeProductVariant_ProductVariantsId",
                table: "ProductSizeProductVariant",
                column: "ProductVariantsId");
        }
    }
}
