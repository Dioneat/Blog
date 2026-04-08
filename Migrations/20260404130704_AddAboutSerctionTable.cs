using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog10.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutSerctionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeroTitle = table.Column<string>(type: "TEXT", nullable: false),
                    HeroSubtitle = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    StatsExperience = table.Column<string>(type: "TEXT", nullable: false),
                    StatsChildren = table.Column<string>(type: "TEXT", nullable: false),
                    StatsCertificates = table.Column<string>(type: "TEXT", nullable: false),
                    BioHtml = table.Column<string>(type: "TEXT", nullable: false),
                    EducationHtml = table.Column<string>(type: "TEXT", nullable: false),
                    SkillsHtml = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutPage", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutPage");
        }
    }
}
