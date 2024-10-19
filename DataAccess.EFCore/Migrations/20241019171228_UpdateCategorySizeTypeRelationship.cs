using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategorySizeTypeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SizeValue",
                table: "UserMeasurements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FavoriteSection",
                table: "UserMeasurements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "UserMeasurements",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filters_Categories_CategoryId",
                table: "Filters");

            migrationBuilder.DropIndex(
                name: "IX_Filters_CategoryId",
                table: "Filters");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "SizeValue",
                table: "UserMeasurements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FavoriteSection",
                table: "UserMeasurements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "UserMeasurements",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CategoryFilter",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    FiltersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFilter", x => new { x.CategoryId, x.FiltersId });
                    table.ForeignKey(
                        name: "FK_CategoryFilter_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CategoryFilter_Filters_FiltersId",
                        column: x => x.FiltersId,
                        principalTable: "Filters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SizeTypeId",
                table: "Categories",
                column: "SizeTypeId",
                unique: true,
                filter: "[SizeTypeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFilter_FiltersId",
                table: "CategoryFilter",
                column: "FiltersId");
        }
    }
}
