using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class ModificatorModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Priorities_PriorityId",
                table: "Gifts");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                table: "Gifts",
                newName: "ModificatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Gifts_PriorityId",
                table: "Gifts",
                newName: "IX_Gifts_ModificatorId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Modificators_ModificatorId",
                table: "Gifts",
                column: "ModificatorId",
                principalTable: "Modificators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Modificators_ModificatorId",
                table: "Gifts");

            migrationBuilder.DropTable(
                name: "Modificators");

            migrationBuilder.RenameColumn(
                name: "ModificatorId",
                table: "Gifts",
                newName: "PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_Gifts_ModificatorId",
                table: "Gifts",
                newName: "IX_Gifts_PriorityId");

            migrationBuilder.CreateTable(
                name: "Priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Priorities_PriorityId",
                table: "Gifts",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
