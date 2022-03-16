using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daylily.Migrations
{
    public partial class SearchBn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OsuUserInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    ModeId = table.Column<string>(type: "TEXT", nullable: false),
                    Group = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestStatus = table.Column<string>(type: "TEXT", nullable: true),
                    RequestLink = table.Column<string>(type: "TEXT", nullable: true),
                    UserPageText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsuUserInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OsuUserInfos_UserId",
                table: "OsuUserInfos",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OsuUserInfos");
        }
    }
}
