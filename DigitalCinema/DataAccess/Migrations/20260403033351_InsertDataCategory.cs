using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalCinema.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Categories (Name) VALUES 
('Action'),
('Comedy'),
('Drama'),
('Horror'),
('Science Fiction'),
('Romance'),
('Thriller'),
('Animation'),
('Documentary');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
