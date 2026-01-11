using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Models.Response._user;

namespace Application.Features._user.Queries.GetUserByIdQueries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Response<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.guid);

            if (user == null)
                return Response<UserResponse>.Fail("Usuario no encontrado.");

            var userDto = _mapper.Map<UserResponse>(user);

            return Response<UserResponse>.Success(userDto);
        }
    }
}
