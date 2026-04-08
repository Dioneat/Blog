using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog10.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldForHeroImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainHeroImageUrl",
                table: "AboutPage",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainHeroImageUrl",
                table: "AboutPage");
        }
    }
}
