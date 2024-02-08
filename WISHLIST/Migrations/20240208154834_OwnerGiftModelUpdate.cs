using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class OwnerGiftModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WishlistId",
                table: "OwnerGifts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OwnerGifts_WishlistId",
                table: "OwnerGifts",
                column: "WishlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerGifts_Wishlists_WishlistId",
                table: "OwnerGifts",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerGifts_Wishlists_WishlistId",
                table: "OwnerGifts");

            migrationBuilder.DropIndex(
                name: "IX_OwnerGifts_WishlistId",
                table: "OwnerGifts");

            migrationBuilder.DropColumn(
                name: "WishlistId",
                table: "OwnerGifts");
        }
    }
}
