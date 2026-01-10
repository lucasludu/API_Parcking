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

    }
}
