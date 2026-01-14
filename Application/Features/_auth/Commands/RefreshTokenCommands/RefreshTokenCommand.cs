using Application.Wrappers;
using MediatR;
using Models.Request._user;
using Models.Response._user;

namespace Application.Features._auth.Commands.RefreshTokenCommands
{
    public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Response<LoginResponse>>;
}
