using Application.Interfaces;
using Application.Wrappers;
using MediatR;

namespace Application.Features._auth.Commands.ForgotPasswordCommands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Response<string>>
    {
        private readonly IAuthService _authService;
        public ForgotPasswordCommandHandler(IAuthService authService) => _authService = authService;

        public async Task<Response<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ForgotPasswordAsync(request.Request);
        }
    }
}
