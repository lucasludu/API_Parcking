using Application.Wrappers;
using Domain.Entities;
using Models.Request._user;
using Models.Response._user;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Response<ApplicationUser>> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken);
        Task<Response<LoginResponse>> LoginUserAsync(AuthLoginRequest request, CancellationToken cancellationToken);
        Task<Response<string>> ConfirmEmailAsync(string userId, string token);
        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<Response<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);

    }
}
