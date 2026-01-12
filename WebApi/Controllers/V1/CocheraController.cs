using Application.Features._cochera.Commands.DeleteCocheraCommands;
using Application.Features._cochera.Commands.UpdateCocheraCommands;
using Application.Features._cochera.Queries.GetCocheraByIdQueries;
using Microsoft.AspNetCore.Mvc;
using Models.Request._cochera;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    //[Authorize]
    public class CocheraController : BaseApiController
    {
        [HttpGet("{guid}")]
        public async Task<IActionResult> GetById(Guid guid)
        {
            var result = await Mediator.Send(new GetCocheraByIdQuery(guid));
            
            return result.Succeeded 
                ? Ok(result) 
                : NotFound(result);
        }

        [HttpPut("{guid}")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] UpdateCocheraRequest request)
        {
            // Combinamos ID de URL con datos del Body
            var result = await Mediator.Send(new UpdateCocheraCommand(guid, request));
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{guid}")]
        // [Authorize(Roles = "Admin")] // Opcional: Solo Admins pueden borrar
        public async Task<IActionResult> Delete(Guid guid)
        {
            var result = await Mediator.Send(new DeleteCocheraCommand(guid));
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
