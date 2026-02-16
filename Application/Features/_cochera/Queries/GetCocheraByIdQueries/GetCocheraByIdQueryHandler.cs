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
            // 1. Obtener ID del Token
            var userIdString = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userIdString))
                return Response<CocheraResponse>.Fail("No estás autenticado.");

            // 2. Buscar al Usuario (con validación de nulidad)
            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return Response<CocheraResponse>.Fail("El usuario no existe en la base de datos.");

            // 3. Validar si tiene Cochera asignada
            // Asumo que CocheraId es Guid? (nullable) o Guid.Empty si no tiene.
            if (user.CocheraId == null || user.CocheraId == Guid.Empty)
                return Response<CocheraResponse>.Fail("Este usuario no tiene una cochera asignada.");

            // 4. Buscar la Cochera
            // Pasamos el Guid directo, SIN .ToString()
            var cochera = await _repository.GetByIdAsync(user.CocheraId, cancellationToken);

            if (cochera == null)
                return Response<CocheraResponse>.Fail("La cochera asignada no se encontró (¿fue eliminada?).");

            // 5. Mapear y devolver
            var dto = _mapper.Map<CocheraResponse>(cochera);
            return Response<CocheraResponse>.Success(dto);
        }
    }
}
