using Application.Features._auth.Commands.ConfirmEmailCommands;
using Application.Features._auth.Commands.ForgotPasswordCommands;
using Application.Features._auth.Commands.LoginCommands;
using Application.Features._auth.Commands.RefreshTokenCommands;
using Application.Features._auth.Commands.RegisterUserCommands;
using Application.Features._auth.Commands.ResetPasswordCommands;
using Application.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Models.Request._user;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "authentication")]
    public class AuthController : BaseApiController
    {
        /// <summary>
        /// Confirm email of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            // Validamos que vengan los datos
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new Response<string>("Parámetros inválidos para confirmación."));

            var result = await Mediator.Send(new ConfirmEmailCommand(userId, token));

            return (!result.Succeeded)
                ? BadRequest(result)
                : Ok(result);
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            var result = await Mediator.Send(new LoginCommand(request));

            return (!result.Succeeded)
                ? Unauthorized(result)
                : Ok(result);
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var result = await Mediator.Send(new RegisterUserCommand(request));

            return (!result.Succeeded)
                ? BadRequest(result)
                : Ok(result);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await Mediator.Send(new ForgotPasswordCommand(request));
            
            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await Mediator.Send(new ResetPasswordCommand(request));
           
            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await Mediator.Send(new RefreshTokenCommand(request));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }
    }
}
