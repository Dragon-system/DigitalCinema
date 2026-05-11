using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelShowSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowMovieHalls_Halls_HallId",
                table: "ShowMovieHalls");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_ShowMovieHalls_ShowMovieHallId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ShowMovieHalls");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "ShowMovieHalls");

            migrationBuilder.AlterColumn<int>(
                name: "ShowMovieHallId",
                table: "Shows",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowMovieHalls_Halls_HallId",
                table: "ShowMovieHalls",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_ShowMovieHalls_ShowMovieHallId",
                table: "Shows",
                column: "ShowMovieHallId",
                principalTable: "ShowMovieHalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowMovieHalls_Halls_HallId",
                table: "ShowMovieHalls");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_ShowMovieHalls_ShowMovieHallId",
                table: "Shows");

            migrationBuilder.AlterColumn<int>(
                name: "ShowMovieHallId",
                table: "Shows",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ShowMovieHalls_Halls_HallId",
                table: "ShowMovieHalls",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_ShowMovieHalls_ShowMovieHallId",
                table: "Shows",
                column: "ShowMovieHallId",
                principalTable: "ShowMovieHalls",
                principalColumn: "Id");
        }
    }
}
