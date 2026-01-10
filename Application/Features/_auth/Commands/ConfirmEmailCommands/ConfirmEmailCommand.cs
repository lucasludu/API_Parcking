using Application.Wrappers;
using MediatR;

namespace Application.Features._auth.Commands.ConfirmEmailCommands
{
    public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Response<string>>;
}
