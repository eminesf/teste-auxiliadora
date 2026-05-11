using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Application.UseCases.Auth;

public class RegisterUseCase(
    IClientRepository clientRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork)
{
  public async Task<AuthResponse> ExecuteAsync(RegisterRequest request)
  {
    if (await clientRepository.EmailExistsAsync(request.Email))
      throw new InvalidOperationException(
          $"Já existe um cliente cadastrado com o email '{request.Email}'.");

    var cleanDocument = new string(request.Document.Where(char.IsDigit).ToArray());
    if (await clientRepository.DocumentExistsAsync(cleanDocument))
      throw new InvalidOperationException(
          $"Já existe um cliente cadastrado com o documento '{request.Document}'.");

    // Todo usuário nasce como User — AdminMaster é criado manualmente no banco
    var client = new Client(
        request.Name,
        request.Email,
        request.Document,
        request.Password,
        ClientRole.User
    );

    await clientRepository.AddAsync(client);
    await unitOfWork.CommitAsync();

    var token = tokenService.GenerateToken(client);

    return new AuthResponse(client.Id, client.Name, client.Email, client.Role, token);
  }
}