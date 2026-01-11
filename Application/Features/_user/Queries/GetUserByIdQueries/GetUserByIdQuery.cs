using Application.Wrappers;
using MediatR;
using Models.Response._user;

namespace Application.Features._user.Queries.GetUserByIdQueries
{
    public record GetUserByIdQuery(string guid) : IRequest<Response<UserResponse>>;
}
