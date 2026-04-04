using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataCinema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Cinemas (Name, Img) VALUES 
('Grand Cinema', 'grand_cinema.png'),
('Galaxy Cinema', 'galaxy_cinema.png'),
('Star Cinema', 'star_cinema.png'),
('Royal Cinema', 'royal_cinema.png'),
('Empire Cinema', 'empire_cinema.png');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
