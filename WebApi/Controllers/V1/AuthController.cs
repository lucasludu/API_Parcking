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
        /// Confirma la dirección de correo electrónico de un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="token">Token de confirmación.</param>
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
        /// Inicia sesión y obtiene un Token JWT.
        /// </summary>
        /// <param name="request">Credenciales de acceso (Email y Password).</param>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            var result = await Mediator.Send(new LoginCommand(request));

            return (!result.Succeeded)
                ? Unauthorized(result)
                : Ok(result);
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">Datos del usuario (Nombre, Email, Password, etc).</param>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var result = await Mediator.Send(new RegisterUserCommand(request));

            return (!result.Succeeded)
                ? BadRequest(result)
                : Ok(result);
        }

        /// <summary>
        /// Solicita el restablecimiento de contraseña (olvidó su clave).
        /// </summary>
        /// <param name="request">Email del usuario.</param>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await Mediator.Send(new ForgotPasswordCommand(request));
            
            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Restablece la contraseña usando un token.
        /// </summary>
        /// <param name="request">Token y nueva contraseña.</param>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await Mediator.Send(new ResetPasswordCommand(request));
           
            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Refresca un Token JWT expirado.
        /// </summary>
        /// <param name="request">Token expirado y Refresh Token.</param>
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
