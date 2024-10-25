using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSizingSystemTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_ProductTypes_ProductTypeId",
                table: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductSizes");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "ProductSizes",
                newName: "SizeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizes_ProductTypeId",
                table: "ProductSizes",
                newName: "IX_ProductSizes_SizeTypeId");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "ProductTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeTypeId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_TypeId",
                table: "ProductTypes",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories",
                column: "SizeTypeId",
                filter: "[SizeTypeId] IS NOT NULL");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_TypeId",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "SizeTypeId",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "SizeTypeId",
                table: "ProductSizes",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizes_SizeTypeId",
                table: "ProductSizes",
                newName: "IX_ProductSizes_ProductTypeId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_ProductTypes_ProductTypeId",
                table: "ProductSizes",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id");
        }
    }
}
