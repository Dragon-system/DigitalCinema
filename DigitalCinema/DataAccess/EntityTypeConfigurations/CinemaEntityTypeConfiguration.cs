using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalCinema.DataAccess.EntityTypeConfigurations
{
    public class CinemaEntityTypeConfiguration : IEntityTypeConfiguration<Cinema>
    {
        public void Configure(EntityTypeBuilder<Cinema> builder)
        {
            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
