using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models.Response._user;

namespace Application.Features._user.Queries.GetProfileQueries
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Response<UserResponse>>

    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProfileQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Response<UserResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var userIdString = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userIdString))
                return Response<UserResponse>.Fail("Token inválido o sin identidad.");

            // Buscamos al usuario por el ID que venía en el Token
            var user = await _userManager.FindByIdAsync(userIdString);

            if (user == null)
                return Response<UserResponse>.Fail("Usuario no encontrado (Token inválido o usuario eliminado).");

            // Mapeamos a DTO (Usando el UserDto que creamos antes)
            var userDto = _mapper.Map<UserResponse>(user);

            return Response<UserResponse>.Success(userDto);
        }
    }
}
