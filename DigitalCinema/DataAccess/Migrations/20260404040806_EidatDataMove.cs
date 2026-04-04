using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class EidatDataMove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Movies (Name, Description, MainImg, Price, ShowTime, CategoryId, CinemaId) VALUES
('El-Kenz', 'A thrilling Egyptian adventure movie.', 'el_kenz.png', 50.00, '2026-04-05 18:30:00', 1, 13),
('Asal Eswed', 'Drama movie featuring intense emotional scenes.', 'asal_eswed.png', 45.00, '2026-04-06 20:00:00', 3, 15),
('Masr Online', 'Comedy/action movie about Egyptian digital life.', 'masr_online.png', 50.00, '2026-04-07 19:00:00', 2, 21);
");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
