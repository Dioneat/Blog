using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog10.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrinciplesHtml",
                table: "AboutPage",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuoteAuthor",
                table: "AboutPage",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuoteText",
                table: "AboutPage",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrinciplesHtml",
                table: "AboutPage");

            migrationBuilder.DropColumn(
                name: "QuoteAuthor",
                table: "AboutPage");

            migrationBuilder.DropColumn(
                name: "QuoteText",
                table: "AboutPage");
        }
    }
}
