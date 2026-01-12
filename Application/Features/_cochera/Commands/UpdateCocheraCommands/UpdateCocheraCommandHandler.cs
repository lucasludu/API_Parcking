using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features._cochera.Commands.UpdateCocheraCommands
{
    public class UpdateCocheraCommandHandler : IRequestHandler<UpdateCocheraCommand, Response<Guid>>
    {
        private readonly IRepositoryAsync<Cochera> _repository;

        public UpdateCocheraCommandHandler(IRepositoryAsync<Cochera> repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(UpdateCocheraCommand request, CancellationToken cancellationToken)
        {
            var cochera = await _repository.GetByIdAsync(request.guid, cancellationToken);

            if (cochera == null)
                return Response<Guid>.Fail("Cochera no encontrada.");

            // Actualizamos solo lo que viene en el request
            if (request.request.Nombre != null) 
                cochera.Nombre = request.request.Nombre;
            if (request.request.Direccion != null) 
                cochera.Direccion = request.request.Direccion;
            if (request.request.CapacidadTotal != null) 
                cochera.CapacidadTotal = request.request.CapacidadTotal.Value;
            if (request.request.ImagenUrl != null) 
                cochera.ImagenUrl = request.request.ImagenUrl;

            await _repository.UpdateAsync(cochera, cancellationToken);

            return Response<Guid>.Success(cochera.Id, "Cochera actualizada correctamente.");
        }
    }
}
