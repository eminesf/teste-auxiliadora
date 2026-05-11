using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.UseCases.Auth;

namespace RentalPipeline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    RegisterUseCase registerUseCase,
    LoginUseCase loginUseCase,
    MeUseCase meUseCase) : ControllerBase
{
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request)
  {
    var response = await registerUseCase.ExecuteAsync(request);
    return CreatedAtAction(nameof(Me), response);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    var response = await loginUseCase.ExecuteAsync(request);
    return Ok(response);
  }

  [HttpGet("me")]
  [Authorize]
  public async Task<IActionResult> Me()
  {
    var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var response = await meUseCase.ExecuteAsync(clientId);
    return Ok(response);
  }
}