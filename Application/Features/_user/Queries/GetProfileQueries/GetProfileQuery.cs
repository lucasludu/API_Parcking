using Application.Wrappers;
using MediatR;
using Models.Response._user;

namespace Application.Features._user.Queries.GetProfileQueries
{
    public record GetProfileQuery(string guid) : IRequest<Response<UserResponse>>;
}
