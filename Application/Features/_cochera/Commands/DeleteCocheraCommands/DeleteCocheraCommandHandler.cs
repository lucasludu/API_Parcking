using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features._cochera.Commands.DeleteCocheraCommands
{
    public class DeleteCocheraCommandHandler : IRequestHandler<DeleteCocheraCommand, Response<Guid>>
    {
        private readonly IRepositoryAsync<Cochera> _repository;

        public DeleteCocheraCommandHandler(IRepositoryAsync<Cochera> repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(DeleteCocheraCommand request, CancellationToken cancellationToken)
        {
            var cochera = await _repository.GetByIdAsync(request.guid, cancellationToken);
           
            if (cochera == null) 
                return Response<Guid>.Fail("Cochera no encontrada.");

            await _repository.DeleteAsync(cochera, cancellationToken);
            return Response<Guid>.Success(request.guid, "Cochera eliminada correctamente.");
        }
    }
}
