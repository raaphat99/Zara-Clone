using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdFieldNamingConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartID",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantID",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserID",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryID",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWishlist_Products_ProductsID",
                table: "ProductWishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWishlist_Wishlists_WishlistsID",
                table: "ProductWishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMeasurements_AspNetUsers_UserID",
                table: "UserMeasurements");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserID",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_UserID",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserID",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Wishlists",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Wishlists",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserMeasurements",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "UserMeasurements",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserMeasurements_UserID",
                table: "UserMeasurements",
                newName: "IX_UserMeasurements_UserId");

            migrationBuilder.RenameColumn(
                name: "WishlistsID",
                table: "ProductWishlist",
                newName: "WishlistsId");

            migrationBuilder.RenameColumn(
                name: "ProductsID",
                table: "ProductWishlist",
                newName: "ProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductWishlist_WishlistsID",
                table: "ProductWishlist",
                newName: "IX_ProductWishlist_WishlistsId");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Products",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameColumn(
                name: "ParentCategoryID",
                table: "Categories",
                newName: "ParentCategoryId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Categories",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ParentCategoryID",
                table: "Categories",
                newName: "IX_Categories_ParentCategoryId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Carts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Carts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductVariantID",
                table: "CartItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameColumn(
                name: "CartID",
                table: "CartItems",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CartItems",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductVariantID",
                table: "CartItems",
                newName: "IX_CartItems_ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartID",
                table: "CartItems",
                newName: "IX_CartItems_CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWishlist_Products_ProductsId",
                table: "ProductWishlist",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWishlist_Wishlists_WishlistsId",
                table: "ProductWishlist",
                column: "WishlistsId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMeasurements_AspNetUsers_UserId",
                table: "UserMeasurements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserId",
                table: "Wishlists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWishlist_Products_ProductsId",
                table: "ProductWishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWishlist_Wishlists_WishlistsId",
                table: "ProductWishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMeasurements_AspNetUsers_UserId",
                table: "UserMeasurements");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserId",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Wishlists",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Wishlists",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserMeasurements",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserMeasurements",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_UserMeasurements_UserId",
                table: "UserMeasurements",
                newName: "IX_UserMeasurements_UserID");

            migrationBuilder.RenameColumn(
                name: "WishlistsId",
                table: "ProductWishlist",
                newName: "WishlistsID");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "ProductWishlist",
                newName: "ProductsID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductWishlist_WishlistsId",
                table: "ProductWishlist",
                newName: "IX_ProductWishlist_WishlistsID");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Products",
                newName: "CategoryID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryID");

            migrationBuilder.RenameColumn(
                name: "ParentCategoryId",
                table: "Categories",
                newName: "ParentCategoryID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                newName: "IX_Categories_ParentCategoryID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Carts",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Carts",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "CartItems",
                newName: "ProductVariantID");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "CartItems",
                newName: "CartID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CartItems",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItems",
                newName: "IX_CartItems_ProductVariantID");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                newName: "IX_CartItems_CartID");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserID",
                table: "Wishlists",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserID",
                table: "Carts",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartID",
                table: "CartItems",
                column: "CartID",
                principalTable: "Carts",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantID",
                table: "CartItems",
                column: "ProductVariantID",
                principalTable: "ProductVariants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserID",
                table: "Carts",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryID",
                table: "Categories",
                column: "ParentCategoryID",
                principalTable: "Categories",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWishlist_Products_ProductsID",
                table: "ProductWishlist",
                column: "ProductsID",
                principalTable: "Products",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWishlist_Wishlists_WishlistsID",
                table: "ProductWishlist",
                column: "WishlistsID",
                principalTable: "Wishlists",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMeasurements_AspNetUsers_UserID",
                table: "UserMeasurements",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserID",
                table: "Wishlists",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
