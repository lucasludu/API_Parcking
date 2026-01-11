using Application.Wrappers;
using MediatR;
using Models.Request._user;

namespace Application.Features._auth.Commands.ForgotPasswordCommands
{
    public record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<Response<string>>;
}
