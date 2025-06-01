using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 小说漫画阅读器.Migrations
{
    /// <inheritdoc />
    public partial class add_chatterid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChapterId",
                table: "T_MangaChapters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "T_MangaChapters");
        }
    }
}
