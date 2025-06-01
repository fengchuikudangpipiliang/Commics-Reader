using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 小说漫画阅读器.Migrations
{
    /// <inheritdoc />
    public partial class add_imgfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverFileName",
                table: "T_MangaWithUserConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "T_MangaChapters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "T_MangaChapters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "T_ImgFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MangaChapterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ImgFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_ImgFiles_T_MangaChapters_MangaChapterId",
                        column: x => x.MangaChapterId,
                        principalTable: "T_MangaChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_ImgFiles_MangaChapterId",
                table: "T_ImgFiles",
                column: "MangaChapterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_ImgFiles");

            migrationBuilder.DropColumn(
                name: "CoverFileName",
                table: "T_MangaWithUserConfigs");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                table: "T_MangaChapters");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "T_MangaChapters");
        }
    }
}
