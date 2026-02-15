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
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetCocheraByIdQueryHandler(IRepositoryAsync<Cochera> repository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Response<CocheraResponse>> Handle(GetCocheraByIdQuery request, CancellationToken cancellationToken)
        {
            var userIdString = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userIdString))
                return Response<CocheraResponse>.Fail("Token inválido o sin identidad.");

            var cochera = await _repository.GetByIdAsync(userIdString, cancellationToken);

            if (cochera == null)
                return Response<CocheraResponse>.Fail("Cochera no encontrada.");

            var dto = _mapper.Map<CocheraResponse>(cochera);
            return Response<CocheraResponse>.Success(dto);
        }
    }
}
