using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class CocheraConfiguration : IEntityTypeConfiguration<Cochera>
    {
        public void Configure(EntityTypeBuilder<Cochera> builder)
        {
            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Direccion)
                .HasMaxLength(250);
            
            // Relación con Tarifas (1 a Muchos) borrar en cascada
            builder.HasMany(c => c.Tarifas)
                .WithOne(t => t.Cochera)
                .HasForeignKey(t => t.CocheraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Tickets (1 a Muchos) no borrar
            builder.HasMany(c => c.Tickets)
                .WithOne(t => t.Cochera)
                .HasForeignKey(t => t.CocheraId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
