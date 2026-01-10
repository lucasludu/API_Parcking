using Application.Interfaces;
using Application.Wrappers;
using MediatR;

namespace Application.Features._auth.Commands.ConfirmEmailCommands
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Response<string>>
    {
        private readonly IAuthService _authService;

        public ConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ConfirmEmailAsync(request.UserId, request.Token);
        }
    }
}
