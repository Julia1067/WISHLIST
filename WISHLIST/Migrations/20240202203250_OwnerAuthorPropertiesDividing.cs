using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class OwnerAuthorPropertiesDividing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Gifts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OwnerGifts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GiftId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerGifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnerGifts_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OwnerGifts_Gifts_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gifts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OwnerWishlists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WishlistId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerWishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnerWishlists_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OwnerWishlists_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_AuthorId",
                table: "Gifts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerGifts_GiftId",
                table: "OwnerGifts",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerGifts_OwnerId",
                table: "OwnerGifts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerWishlists_OwnerId",
                table: "OwnerWishlists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerWishlists_WishlistId",
                table: "OwnerWishlists",
                column: "WishlistId",
                unique: true,
                filter: "[WishlistId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_AspNetUsers_AuthorId",
                table: "Gifts",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_AspNetUsers_AuthorId",
                table: "Gifts");

            migrationBuilder.DropTable(
                name: "OwnerGifts");

            migrationBuilder.DropTable(
                name: "OwnerWishlists");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_AuthorId",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Gifts");
        }
    }
}
