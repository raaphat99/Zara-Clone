using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductSizesTableToSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_SizeTypes_SizeTypeId",
                table: "ProductSizes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductSizes_ProductSizeId",
                table: "ProductVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSizes",
                table: "ProductSizes");

            migrationBuilder.RenameTable(
                name: "ProductSizes",
                newName: "Sizes");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizes_SizeTypeId",
                table: "Sizes",
                newName: "IX_Sizes_SizeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sizes",
                table: "Sizes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Sizes_ProductSizeId",
                table: "ProductVariants",
                column: "ProductSizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Sizes_SizeTypes_SizeTypeId",
                table: "Sizes",
                column: "SizeTypeId",
                principalTable: "SizeTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Sizes_ProductSizeId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_Sizes_SizeTypes_SizeTypeId",
                table: "Sizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sizes",
                table: "Sizes");

            migrationBuilder.RenameTable(
                name: "Sizes",
                newName: "ProductSizes");

            migrationBuilder.RenameIndex(
                name: "IX_Sizes_SizeTypeId",
                table: "ProductSizes",
                newName: "IX_ProductSizes_SizeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSizes",
                table: "ProductSizes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_SizeTypes_SizeTypeId",
                table: "ProductSizes",
                column: "SizeTypeId",
                principalTable: "SizeTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductSizes_ProductSizeId",
                table: "ProductVariants",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
