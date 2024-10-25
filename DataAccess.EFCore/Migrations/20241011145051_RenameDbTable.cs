using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameDbTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_ProductTypes_SizeTypeId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_ProductTypes_SizeTypeId",
                table: "ProductSizes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypes_ProductTypes_TypeId",
                table: "ProductTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTypes",
                table: "ProductTypes");

            migrationBuilder.RenameTable(
                name: "ProductTypes",
                newName: "SizeTypes");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTypes_TypeId",
                table: "SizeTypes",
                newName: "IX_SizeTypes_TypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SizeTypes",
                table: "SizeTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_SizeTypes_SizeTypeId",
                table: "Categories",
                column: "SizeTypeId",
                principalTable: "SizeTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_SizeTypes_SizeTypeId",
                table: "ProductSizes",
                column: "SizeTypeId",
                principalTable: "SizeTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeTypes_SizeTypes_TypeId",
                table: "SizeTypes",
                column: "TypeId",
                principalTable: "SizeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_SizeTypes_SizeTypeId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_SizeTypes_SizeTypeId",
                table: "ProductSizes");

            migrationBuilder.DropForeignKey(
                name: "FK_SizeTypes_SizeTypes_TypeId",
                table: "SizeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SizeTypes",
                table: "SizeTypes");

            migrationBuilder.RenameTable(
                name: "SizeTypes",
                newName: "ProductTypes");

            migrationBuilder.RenameIndex(
                name: "IX_SizeTypes_TypeId",
                table: "ProductTypes",
                newName: "IX_ProductTypes_TypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTypes",
                table: "ProductTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_ProductTypes_SizeTypeId",
                table: "Categories",
                column: "SizeTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_ProductTypes_SizeTypeId",
                table: "ProductSizes",
                column: "SizeTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypes_ProductTypes_TypeId",
                table: "ProductTypes",
                column: "TypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
