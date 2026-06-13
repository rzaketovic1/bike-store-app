using Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/health")]
[AllowAnonymous]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a lightweight health response.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthCheckResponse> Get()
    {
        return Ok(new HealthCheckResponse
        {
            Timestamp = DateTime.UtcNow.ToString("O")
        });
    }
}
