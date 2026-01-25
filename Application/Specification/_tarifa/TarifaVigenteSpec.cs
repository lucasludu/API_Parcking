using Ardalis.Specification;
using Domain.Entities;
using Domain.Enums;

namespace Application.Specification._tarifa
{
    public class TarifaVigenteSpec : Specification<Tarifa>
    {
        public TarifaVigenteSpec(Guid cocheraId, TipoVehiculo tipoVehiculo)
        {
            Query
                .Where(t => t.CocheraId == cocheraId && t.TipoVehiculo == tipoVehiculo);
        }
    }
}
