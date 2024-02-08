using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class ModificatorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Modificators_ModificatorId",
                table: "Gifts");

            migrationBuilder.DropTable(
                name: "Modificators");

            migrationBuilder.DropIndex(
                name: "IX_OwnerWishlists_WishlistId",
                table: "OwnerWishlists");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_ModificatorId",
                table: "Gifts");

            migrationBuilder.RenameColumn(
                name: "ModificatorId",
                table: "Gifts",
                newName: "ModificatorType");

            migrationBuilder.AddColumn<int>(
                name: "ModificatorType",
                table: "Wishlists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OwnerWishlists_WishlistId",
                table: "OwnerWishlists",
                column: "WishlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OwnerWishlists_WishlistId",
                table: "OwnerWishlists");

            migrationBuilder.DropColumn(
                name: "ModificatorType",
                table: "Wishlists");

            migrationBuilder.RenameColumn(
                name: "ModificatorType",
                table: "Gifts",
                newName: "ModificatorId");

            migrationBuilder.CreateTable(
                name: "Modificators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modificators", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerWishlists_WishlistId",
                table: "OwnerWishlists",
                column: "WishlistId",
                unique: true,
                filter: "[WishlistId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_ModificatorId",
                table: "Gifts",
                column: "ModificatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Modificators_ModificatorId",
                table: "Gifts",
                column: "ModificatorId",
                principalTable: "Modificators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
