using Chat.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers;

[Route("[controller]")]
public class PrivateResourcesController
    : ControllerBase
{
    [BasicAuth]
    [HttpGet("basic-auth")]
    public IActionResult BasicAuthSample()
    {
        return Ok("If you see this, you are authenticated with basic authentication");
    }

    [Authorize]
    [HttpGet("basic-jwt")]
    public IActionResult BasicJwtSample()
    {
        return Ok($"If you see this, you are authenticated. Your claims: {string.Join(",",User.Claims)}");
    }

    [Authorize("policy-claim-scope-required")]
    [HttpGet("advanced-jwt")]
    public IActionResult AdvancedJwtSample()
    {
        return Ok($"If you see this, you are authenticated and authorized with expected scope claim. Your claims: {string.Join(",", User.Claims)}");
    }
}
