using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class DropIteractionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interactions_AspNetUsers_ApplicationUserId",
                table: "Interactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Interactions",
                table: "Interactions");

            migrationBuilder.RenameTable(
                name: "Interactions",
                newName: "InteractionModel");

            migrationBuilder.RenameIndex(
                name: "IX_Interactions_ApplicationUserId",
                table: "InteractionModel",
                newName: "IX_InteractionModel_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InteractionModel",
                table: "InteractionModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InteractionModel_AspNetUsers_ApplicationUserId",
                table: "InteractionModel",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InteractionModel_AspNetUsers_ApplicationUserId",
                table: "InteractionModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InteractionModel",
                table: "InteractionModel");

            migrationBuilder.RenameTable(
                name: "InteractionModel",
                newName: "Interactions");

            migrationBuilder.RenameIndex(
                name: "IX_InteractionModel_ApplicationUserId",
                table: "Interactions",
                newName: "IX_Interactions_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interactions",
                table: "Interactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interactions_AspNetUsers_ApplicationUserId",
                table: "Interactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
