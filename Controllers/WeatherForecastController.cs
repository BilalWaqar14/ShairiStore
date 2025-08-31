using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShairiStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
        };

        [HttpGet("public")]
        public IActionResult Public() => Ok("This endpoint is public");

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            var name = User.Identity?.Name ?? "unknown";
            return Ok(new { message = "You are authorized", user = name, claims = User.Claims.Select(c => new { c.Type, c.Value }) });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly() => Ok("Only Admin role can call this");
    }
}
