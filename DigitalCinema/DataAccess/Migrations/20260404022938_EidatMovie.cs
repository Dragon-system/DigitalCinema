using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class EidatMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Img",
                table: "Movies",
                newName: "MainImg");

            migrationBuilder.AddColumn<string>(
                name: "SaubImg",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaubImg",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "MainImg",
                table: "Movies",
                newName: "Img");
        }
    }
}
