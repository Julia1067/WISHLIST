using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class InteractionModelModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interactions_AspNetUsers_FirstUserId",
                table: "Interactions");

            migrationBuilder.RenameColumn(
                name: "FirstUserId",
                table: "Interactions",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Interactions_FirstUserId",
                table: "Interactions",
                newName: "IX_Interactions_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interactions_AspNetUsers_ApplicationUserId",
                table: "Interactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interactions_AspNetUsers_ApplicationUserId",
                table: "Interactions");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Interactions",
                newName: "FirstUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Interactions_ApplicationUserId",
                table: "Interactions",
                newName: "IX_Interactions_FirstUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interactions_AspNetUsers_FirstUserId",
                table: "Interactions",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
