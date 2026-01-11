using Application.Wrappers;
using MediatR;
using Models.Response._user;

namespace Application.Features._user.Queries.GetAllUsersQueries
{
    public record GetAllUsersQuery(GetAllUsersParameters Parameters) : IRequest<PagedResponse<List<UserResponse>>>;
}
