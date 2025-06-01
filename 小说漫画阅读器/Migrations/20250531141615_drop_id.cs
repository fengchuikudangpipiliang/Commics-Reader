using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 小说漫画阅读器.Migrations
{
    /// <inheritdoc />
    public partial class drop_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_ImgFiles_T_MangaChapters_MangaChapterId",
                table: "T_ImgFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_MangaChapters",
                table: "T_MangaChapters");

            migrationBuilder.DropIndex(
                name: "IX_T_ImgFiles_MangaChapterId",
                table: "T_ImgFiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "T_MangaChapters");

            migrationBuilder.DropColumn(
                name: "MangaChapterId",
                table: "T_ImgFiles");

            migrationBuilder.AlterColumn<string>(
                name: "ChapterId",
                table: "T_MangaChapters",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MangaChapterChapterId",
                table: "T_ImgFiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_MangaChapters",
                table: "T_MangaChapters",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_T_ImgFiles_MangaChapterChapterId",
                table: "T_ImgFiles",
                column: "MangaChapterChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_ImgFiles_T_MangaChapters_MangaChapterChapterId",
                table: "T_ImgFiles",
                column: "MangaChapterChapterId",
                principalTable: "T_MangaChapters",
                principalColumn: "ChapterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_ImgFiles_T_MangaChapters_MangaChapterChapterId",
                table: "T_ImgFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_MangaChapters",
                table: "T_MangaChapters");

            migrationBuilder.DropIndex(
                name: "IX_T_ImgFiles_MangaChapterChapterId",
                table: "T_ImgFiles");

            migrationBuilder.DropColumn(
                name: "MangaChapterChapterId",
                table: "T_ImgFiles");

            migrationBuilder.AlterColumn<string>(
                name: "ChapterId",
                table: "T_MangaChapters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "T_MangaChapters",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "MangaChapterId",
                table: "T_ImgFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_MangaChapters",
                table: "T_MangaChapters",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_T_ImgFiles_MangaChapterId",
                table: "T_ImgFiles",
                column: "MangaChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_ImgFiles_T_MangaChapters_MangaChapterId",
                table: "T_ImgFiles",
                column: "MangaChapterId",
                principalTable: "T_MangaChapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
