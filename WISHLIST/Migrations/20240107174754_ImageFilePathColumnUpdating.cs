using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WISHLIST.Migrations
{
    /// <inheritdoc />
    public partial class ImageFilePathColumnUpdating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contentViews");

            migrationBuilder.RenameColumn(
                name: "ImageFileName",
                table: "AspNetUsers",
                newName: "ImageFilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFilePath",
                table: "AspNetUsers",
                newName: "ImageFileName");

            migrationBuilder.CreateTable(
                name: "contentViews",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contentViews", x => x.ID);
                });
        }
    }
}
