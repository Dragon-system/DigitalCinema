using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalCinema.DataAccess.EntityTypeConfigurations
{
    public class MovieEntityTypeConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.Property(e => e.Name)
                .HasColumnType("varchar(200)")
                .IsRequired();
            builder.Property(e => e.Description)
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder.Property(e => e.MainImg)
               .HasColumnType("varchar(Max)")
                .IsRequired();
            builder.Property(e => e.ShowTime)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(e => e.Price)
                .HasColumnType("decimal(10,2)");
        }
    }
}
