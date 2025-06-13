using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobMasterApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFilepathToResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Resumes");
        }
    }
}
