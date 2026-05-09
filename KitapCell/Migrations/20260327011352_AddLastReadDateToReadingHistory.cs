using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitapCell.Migrations
{
    /// <inheritdoc />
    public partial class AddLastReadDateToReadingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadDate",
                table: "ReadingHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalReadSeconds",
                table: "ReadingHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadDate",
                table: "ReadingHistories");

            migrationBuilder.DropColumn(
                name: "TotalReadSeconds",
                table: "ReadingHistories");
        }
    }
}
