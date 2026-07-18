using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specification._cochera
{
    public class GetCocheraByIdSpec : Specification<Cochera>
    {
        public GetCocheraByIdSpec(Guid cocheraId)
        {
            Query.Where(c => c.Id == cocheraId)
                 .Include(c => c.Lugares)
                 .AsNoTracking();
        }
    }
}
