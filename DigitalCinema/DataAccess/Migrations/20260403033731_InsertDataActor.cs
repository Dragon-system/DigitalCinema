using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataActor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Actors (Name, Img, Role) VALUES
('Ahmed Helmy', 'ahmed_helmy.png', 'Comedy Lead'),
('Hend Sabry', 'hend_sabry.png', 'Drama Lead'),
('Ahmed Ezz', 'ahmed_ezz.png', 'Action Lead'),
('Mona Zaki', 'mona_zaki.png', 'Romantic Lead'),
('Yasser Galal', 'yasser_galal.png', 'Thriller Lead'),
('Karim Abdel Aziz', 'karim_abdelaziz.png', 'Action/Comedy Lead'),
('Ghada Adel', 'ghada_adel.png', 'Romantic Lead'),
('Mohamed Henedy', 'mohamed_henedy.png', 'Comedy Lead');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
