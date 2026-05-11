using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;

namespace RentalPipeline.Application.UseCases.Auth;

public class MeUseCase(IClientRepository clientRepository)
{
  public async Task<ClientResponse> ExecuteAsync(Guid clientId)
  {
    var client = await clientRepository.GetByIdAsync(clientId)
        ?? throw new KeyNotFoundException("Usuário não encontrado.");

    return ClientResponse.FromEntity(client);
  }
}