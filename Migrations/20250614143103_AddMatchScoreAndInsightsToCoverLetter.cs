using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobMasterApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchScoreAndInsightsToCoverLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Insights",
                table: "CoverLetters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchScore",
                table: "CoverLetters",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Insights",
                table: "CoverLetters");

            migrationBuilder.DropColumn(
                name: "MatchScore",
                table: "CoverLetters");
        }
    }
}
