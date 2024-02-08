using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class RenewIteractionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InteractionModel_AspNetUsers_ApplicationUserId",
                table: "InteractionModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InteractionModel",
                table: "InteractionModel");

            migrationBuilder.DropIndex(
                name: "IX_InteractionModel_ApplicationUserId",
                table: "InteractionModel");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "InteractionModel");

            migrationBuilder.RenameTable(
                name: "InteractionModel",
                newName: "Interactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interactions",
                table: "Interactions",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Interactions",
                table: "Interactions");

            migrationBuilder.RenameTable(
                name: "Interactions",
                newName: "InteractionModel");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "InteractionModel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InteractionModel",
                table: "InteractionModel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionModel_ApplicationUserId",
                table: "InteractionModel",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InteractionModel_AspNetUsers_ApplicationUserId",
                table: "InteractionModel",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
