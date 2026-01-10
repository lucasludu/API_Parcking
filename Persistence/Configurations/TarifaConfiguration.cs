using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class TarifaConfiguration : IEntityTypeConfiguration<Tarifa>
    {
        public void Configure(EntityTypeBuilder<Tarifa> builder)
        {
            builder.Property(t => t.PrecioHora)
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.PrecioEstadia)
                .HasColumnType("decimal(18,2)");
        }
    }
}
