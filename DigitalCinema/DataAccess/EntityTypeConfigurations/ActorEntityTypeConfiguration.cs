using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalCinema.DataAccess.EntityTypeConfigurations
{
    public class ActorEntityTypeConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.Property(e => e.Name)
                .HasColumnType("varchar(200)")
                .IsRequired();
            builder.Property(e => e.Role)
                .HasColumnType("varchar(200)")
                .IsRequired();
            builder.Property(e => e.Img)
                .HasColumnType("varchar(200)")
                .IsRequired();
        }
    }
}
