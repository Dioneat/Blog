using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog10.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashCradsAndQuizTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlashcardSetId",
                table: "Articles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Articles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlashcardSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CardsJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", nullable: false),
                    QuestionsJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_FlashcardSetId",
                table: "Articles",
                column: "FlashcardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_QuizId",
                table: "Articles",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_FlashcardSets_FlashcardSetId",
                table: "Articles",
                column: "FlashcardSetId",
                principalTable: "FlashcardSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Quizzes_QuizId",
                table: "Articles",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_FlashcardSets_FlashcardSetId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Quizzes_QuizId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "FlashcardSets");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Articles_FlashcardSetId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_QuizId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "FlashcardSetId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Articles");
        }
    }
}
