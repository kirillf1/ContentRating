using Microsoft.AspNetCore.Mvc;

namespace ContentRatingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ConfigurationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetConfiguration()
    {
        var config = new
        {
            BaseUrl = _configuration["ClientConfiguration:BaseUrl"] ?? "https://localhost:7079",
            SignalRHubUrl = _configuration["ClientConfiguration:SignalRHubUrl"] ?? "https://localhost:7079"
        };

        return Ok(config);
    }
} 