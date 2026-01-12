using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DatabaseFacade Database { get; }
        
        DbSet<Domain.Entities.Cochera> Cocheras { get; set; }
        DbSet<Domain.Entities.Lugar> Lugares { get; set; }
        DbSet<Domain.Entities.Tarifa> Tarifas { get; set; }
        DbSet<Domain.Entities.Ticket> Tickets { get; set; }
    }
}
