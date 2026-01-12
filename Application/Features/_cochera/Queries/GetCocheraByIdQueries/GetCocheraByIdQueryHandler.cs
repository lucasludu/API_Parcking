using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Models.Response._cochera;

namespace Application.Features._cochera.Queries.GetCocheraByIdQueries
{
    public class GetCocheraByIdQueryHandler : IRequestHandler<GetCocheraByIdQuery, Response<CocheraResponse>>
    {
        private readonly IRepositoryAsync<Cochera> _repository;
        private readonly IMapper _mapper;

        public GetCocheraByIdQueryHandler(IRepositoryAsync<Cochera> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<CocheraResponse>> Handle(GetCocheraByIdQuery request, CancellationToken cancellationToken)
        {
            var cochera = await _repository.GetByIdAsync(request.guid, cancellationToken);

            if (cochera == null)
                return Response<CocheraResponse>.Fail("Cochera no encontrada.");

            var dto = _mapper.Map<CocheraResponse>(cochera);
            return Response<CocheraResponse>.Success(dto);
        }
    }
}
