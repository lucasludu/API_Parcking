using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using AutoMapper;

namespace Application.Features._cochera.Commands.UpdateCocheraCommands
{
    public class UpdateCocheraCommandHandler : IRequestHandler<UpdateCocheraCommand, Response<Guid>>
    {
        private readonly IRepositoryAsync<Cochera> _repository;
        private readonly IMapper _mapper;
        private readonly INotifier _notifier;

        public UpdateCocheraCommandHandler(IRepositoryAsync<Cochera> repository, IMapper mapper, INotifier notifier)
        {
            _repository = repository;
            _mapper = mapper;
            _notifier = notifier;
        }

        public async Task<Response<Guid>> Handle(UpdateCocheraCommand request, CancellationToken cancellationToken)
        {
            var cochera = await _repository.GetByIdAsync(request.guid, cancellationToken);

            if (cochera == null)
                return Response<Guid>.Fail("Cochera no encontrada.");

            _mapper.Map(request.request, cochera);

            await _repository.UpdateAsync(cochera, cancellationToken);
            await _notifier.NotifyDashboardUpdate(cochera.Id);

            return Response<Guid>.Success(cochera.Id, "Cochera actualizada correctamente.");
        }
    }
}
