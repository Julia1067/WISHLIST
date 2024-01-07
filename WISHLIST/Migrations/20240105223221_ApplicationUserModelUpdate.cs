using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "AspNetUsers");
        }
    }
}
