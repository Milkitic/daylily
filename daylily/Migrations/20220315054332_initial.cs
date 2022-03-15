using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daylily.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeatmapScans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BeatmapSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Artist = table.Column<string>(type: "TEXT", nullable: true),
                    Mapper = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapScans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    qq = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TokenType = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresIn = table.Column<long>(type: "INTEGER", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.qq);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FavoriteCount = table.Column<long>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<long>(type: "INTEGER", nullable: false),
                    BeatmapScanId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeatmapStats_BeatmapScans_BeatmapScanId",
                        column: x => x.BeatmapScanId,
                        principalTable: "BeatmapScans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapSubscribes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScribeUserId = table.Column<string>(type: "TEXT", nullable: false),
                    BeatmapScanId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapSubscribes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeatmapSubscribes_BeatmapScans_BeatmapScanId",
                        column: x => x.BeatmapScanId,
                        principalTable: "BeatmapScans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapScans_BeatmapSetId",
                table: "BeatmapScans",
                column: "BeatmapSetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapStats_BeatmapScanId",
                table: "BeatmapStats",
                column: "BeatmapScanId");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSubscribes_BeatmapScanId",
                table: "BeatmapSubscribes",
                column: "BeatmapScanId");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSubscribes_ScribeUserId_BeatmapScanId",
                table: "BeatmapSubscribes",
                columns: new[] { "ScribeUserId", "BeatmapScanId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeatmapStats");

            migrationBuilder.DropTable(
                name: "BeatmapSubscribes");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "BeatmapScans");
        }
    }
}
