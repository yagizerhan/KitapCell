using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitapCell.Migrations
{
    /// <inheritdoc />
    public partial class AddEpubLocationCfi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastLocationCfi",
                table: "ReadingHistories",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLocationCfi",
                table: "ReadingHistories");
        }
    }
}
