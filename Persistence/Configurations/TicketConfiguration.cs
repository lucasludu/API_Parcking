using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.Property(t => t.Total)
                .HasColumnType("decimal(18,2)");
            
            builder.Property(t => t.Patente)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}
