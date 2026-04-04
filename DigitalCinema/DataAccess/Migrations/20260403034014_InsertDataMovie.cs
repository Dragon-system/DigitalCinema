using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Movies (Name, Description, Img, Price, ShowTime, CategoryId, CinemaId) VALUES
('El-Kenz', 'A thrilling Egyptian adventure movie.', 'el_kenz.png', 50.00, '2026-04-05 18:30:00', 1, 1),
('Asal Eswed', 'Drama movie featuring intense emotional scenes.', 'asal_eswed.png', 45.00, '2026-04-06 20:00:00', 2, 1),
('Captain Masr', 'Action-packed movie starring top Egyptian actors.', 'captain_masr.png', 60.00, '2026-04-07 17:00:00', 1, 2),
('El-Leila El-Kebira', 'Comedy movie full of Egyptian humor.', 'el_leila_el_kebira.png', 40.00, '2026-04-08 19:30:00', 3, 2),
('The Pharaohs Return', 'Epic historical movie set in ancient Egypt.', 'the_pharaohs_return.png', 70.00, '2026-04-09 16:00:00', 1, 3),
('Love in Cairo', 'Romantic story with a Cairo backdrop.', 'love_in_cairo.png', 35.00, '2026-04-10 21:00:00', 2, 3),
('The Nile Secret', 'Mystery and suspense along the Nile.', 'the_nile_secret.png', 55.00, '2026-04-11 18:00:00', 4, 1),
('Masr Online', 'Comedy/action movie about Egyptian digital life.', 'masr_online.png', 50.00, '2026-04-12 20:30:00', 3, 2);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
