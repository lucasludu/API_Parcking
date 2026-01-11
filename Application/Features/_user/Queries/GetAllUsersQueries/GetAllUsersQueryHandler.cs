using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Response._user;

namespace Application.Features._user.Queries.GetAllUsersQueries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResponse<List<UserResponse>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            // Filtrado (Opcional)
            if (!string.IsNullOrEmpty(request.Parameters.Name))
            {
                usersQuery = usersQuery.Where(u => u.UserName!.Contains(request.Parameters.Name));
            }

            var totalRecords = await usersQuery.CountAsync(cancellationToken);

            var users = await usersQuery
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserResponse>>(users);

            return new PagedResponse<List<UserResponse>>(userDtos, request.Parameters.PageNumber, request.Parameters.PageSize, totalRecords);
        }
    }
}
