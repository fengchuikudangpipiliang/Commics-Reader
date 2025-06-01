using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 小说漫画阅读器.Migrations
{
    /// <inheritdoc />
    public partial class add_mangachapterr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_MangaChapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MangaWithUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_MangaChapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_MangaChapters_T_MangaWithUserConfigs_MangaWithUserId",
                        column: x => x.MangaWithUserId,
                        principalTable: "T_MangaWithUserConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_MangaChapters_MangaWithUserId",
                table: "T_MangaChapters",
                column: "MangaWithUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_MangaChapters");
        }
    }
}
