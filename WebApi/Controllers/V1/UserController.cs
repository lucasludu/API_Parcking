using Application.Features._user.Commands.DeleteUserCommands;
using Application.Features._user.Commands.ToggleUserActiveCommands;
using Application.Features._user.Commands.UpdateUserCommands;
using Application.Features._user.Queries.GetAllUsersQueries;
using Application.Features._user.Queries.GetProfileQueries;
using Application.Features._user.Queries.GetUserByIdQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request._user;
using System.Security.Claims;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "users")]
    public class UserController : BaseApiController
    {

        /// <summary>
        /// Obtiene un usuario específico por su ID (GUID).
        /// </summary>
        /// <param name="guid">ID del usuario.</param>
        [HttpGet("get/{guid}")]
        public async Task<IActionResult> GetUser(string guid)
        {
            var result = await Mediator.Send(new GetUserByIdQuery(guid));
            return result.Succeeded 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Obtiene un listado paginado de usuarios.
        /// </summary>
        /// <param name="request">Parámetros de paginación y filtrado.</param>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersParameters request)
        {
            var result = await Mediator.Send(new GetAllUsersQuery(request));
            return result.Succeeded
                ? Ok(result)
                : NotFound(result);
        }

        /// <summary>
        /// Obtiene el perfil del usuario actualmente autenticado.
        /// </summary>
        [HttpGet("me")] // La ruta será: api/v1/User/me
        [Authorize]     // 🔒 OBLIGATORIO: Solo entra si envía Token válido
        public async Task<IActionResult> GetMyProfile()
        {
         
            var result = await Mediator.Send(new GetProfileQuery());

            return result.Succeeded 
                ? Ok(result) 
                : NotFound(result);
        }

        /// <summary>
        /// Actualiza los datos de un usuario.
        /// </summary>
        /// <param name="guid">ID del usuario a actualizar.</param>
        /// <param name="request">Datos nuevos.</param>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string guid, [FromBody] UpdateUserRequest request)
        {
            var command = new UpdateUserCommand(guid, request);
            var result = await Mediator.Send(command);

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Activa o desactiva un usuario.
        /// </summary>
        /// <param name="guid">ID del usuario.</param>
        [HttpPatch("toggle-active/{guid}")]
        public async Task<IActionResult> ToggleUserActive(string guid)
        {
            var result = await Mediator.Send(new ToggleUserActiveCommand(guid));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Elimina permanentemente un usuario.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await Mediator.Send(new DeleteUserCommand(id));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

    }
}
