using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces;
using RentalPipeline.Application.Interfaces.Repositories;

namespace RentalPipeline.Application.UseCases.Auth;

public class LoginUseCase(
    IClientRepository clientRepository,
    ITokenService tokenService)
{
  public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
  {
    var client = await clientRepository.GetByEmailAsync(request.Email)
        ?? throw new UnauthorizedAccessException("Email ou senha inválidos.");

    if (!client.VerifyPassword(request.Password))
      throw new UnauthorizedAccessException("Email ou senha inválidos.");

    var token = tokenService.GenerateToken(client);

    return new AuthResponse(client.Id, client.Name, client.Email, client.Role, token);
  }
}