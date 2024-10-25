using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class MakeCategorySizeTypeOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories",
                column: "SizeTypeId",
                unique: true,
                filter: "[SizeTypeId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories",
                column: "SizeTypeId");
        }
    }
}
