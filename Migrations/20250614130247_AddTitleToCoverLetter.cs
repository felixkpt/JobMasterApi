using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobMasterApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToCoverLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CoverLetters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "CoverLetters");
        }
    }
}
