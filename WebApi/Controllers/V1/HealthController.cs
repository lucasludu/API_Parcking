using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "health")]
    [AllowAnonymous]
    public class HealthController : BaseApiController
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok("API esta en linea");
        }
    }
}
