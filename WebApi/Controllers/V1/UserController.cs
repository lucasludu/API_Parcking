using Application.Features._user.Commands.DeleteUserCommands;
using Application.Features._user.Commands.ToggleUserActiveCommands;
using Application.Features._user.Commands.UpdateUserCommands;
using Application.Features._user.Queries.GetAllUsersQueries;
using Application.Features._user.Queries.GetProfileQueries;
using Application.Features._user.Queries.GetUserByIdQueries;
using Microsoft.AspNetCore.Mvc;
using Models.Request._user;
using System.Security.Claims;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "users")]
    public class UserController : BaseApiController
    {

        [HttpGet("get/{guid}")]
        public async Task<IActionResult> GetUser([FromBody] string guid)
        {
            var result = await Mediator.Send(new GetUserByIdQuery(guid));
            return result.Succeeded 
                ? Ok(result) 
                : NotFound(result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers([FromBody] GetAllUsersParameters request)
        {
            var result = await Mediator.Send(new GetAllUsersQuery(request));
            return result.Succeeded
                ? Ok(result)
                : NotFound(result);
        }

        [HttpGet("me")] // La ruta será: api/v1/User/me
        //[Authorize]     // 🔒 OBLIGATORIO: Solo entra si envía Token válido
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token inválido o sin identidad.");

            var result = await Mediator.Send(new GetProfileQuery(userId));

            return result.Succeeded 
                ? Ok(result) 
                : NotFound(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string guid, [FromBody] UpdateUserRequest request)
        {
            var command = new UpdateUserCommand(guid, request);
            var result = await Mediator.Send(command);

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        [HttpPatch("toggle-active/{guid}")]
        public async Task<IActionResult> ToggleUserActive(string guid)
        {
            var result = await Mediator.Send(new ToggleUserActiveCommand(guid));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string guid)
        {
            var result = await Mediator.Send(new DeleteUserCommand(guid));

            return result.Succeeded 
                ? Ok(result) 
                : BadRequest(result);
        }

    }
}
