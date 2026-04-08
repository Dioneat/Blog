using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog10.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfAndAudioFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileAttachmentName",
                table: "Articles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileAttachmentUrl",
                table: "Articles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainAudioUrl",
                table: "Articles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainVideoUrl",
                table: "Articles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileAttachmentName",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "FileAttachmentUrl",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MainAudioUrl",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MainVideoUrl",
                table: "Articles");
        }
    }
}
