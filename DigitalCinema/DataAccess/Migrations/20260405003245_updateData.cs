using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class updateData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupImgs_Movies_movieId",
                table: "SupImgs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "SupImgs");

            migrationBuilder.RenameColumn(
                name: "movieId",
                table: "SupImgs",
                newName: "MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_SupImgs_movieId",
                table: "SupImgs",
                newName: "IX_SupImgs_MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupImgs_Movies_MovieId",
                table: "SupImgs",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupImgs_Movies_MovieId",
                table: "SupImgs");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "SupImgs",
                newName: "movieId");

            migrationBuilder.RenameIndex(
                name: "IX_SupImgs_MovieId",
                table: "SupImgs",
                newName: "IX_SupImgs_movieId");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "SupImgs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_SupImgs_Movies_movieId",
                table: "SupImgs",
                column: "movieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
