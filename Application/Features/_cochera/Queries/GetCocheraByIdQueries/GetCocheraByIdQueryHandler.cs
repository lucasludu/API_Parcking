using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models.Response._cochera;

namespace Application.Features._cochera.Queries.GetCocheraByIdQueries
{
    public class GetCocheraByIdQueryHandler : IRequestHandler<GetCocheraByIdQuery, Response<CocheraResponse>>
    {
        private readonly IRepositoryAsync<Cochera> _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetCocheraByIdQueryHandler(IRepositoryAsync<Cochera> repository, IMapper mapper, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Response<CocheraResponse>> Handle(GetCocheraByIdQuery request, CancellationToken cancellationToken)
        {
            var userIdString = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userIdString))
                return Response<CocheraResponse>.Fail("Token inválido o sin identidad.");

            var user = await _userManager.FindByIdAsync(userIdString);

            var cochera = await _repository.GetByIdAsync(user!.Id, cancellationToken);

            if (cochera == null)
                return Response<CocheraResponse>.Fail("Cochera no encontrada.");

            var dto = _mapper.Map<CocheraResponse>(cochera);
            return Response<CocheraResponse>.Success(dto);
        }
    }
}
