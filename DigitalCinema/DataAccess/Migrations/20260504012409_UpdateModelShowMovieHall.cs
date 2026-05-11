using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelShowMovieHall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowMovieHallId",
                table: "Shows",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ShowMovieHalls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TicketPrice",
                table: "ShowMovieHalls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Shows_ShowMovieHallId",
                table: "Shows",
                column: "ShowMovieHallId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_ShowMovieHalls_ShowMovieHallId",
                table: "Shows",
                column: "ShowMovieHallId",
                principalTable: "ShowMovieHalls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shows_ShowMovieHalls_ShowMovieHallId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_ShowMovieHallId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ShowMovieHallId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ShowMovieHalls");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "ShowMovieHalls");
        }
    }
}
