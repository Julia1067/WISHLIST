using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class GiftModelNewFieldsAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Wishlists",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFullfilled",
                table: "Gifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "Gifts",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Gifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_PriorityId",
                table: "Gifts",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Priorities_PriorityId",
                table: "Gifts",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Priorities_PriorityId",
                table: "Gifts");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_PriorityId",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "IsFullfilled",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Gifts");
        }
    }
}
