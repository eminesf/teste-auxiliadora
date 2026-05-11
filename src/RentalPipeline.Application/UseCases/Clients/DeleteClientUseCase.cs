using RentalPipeline.Application.Interfaces.Repositories;

namespace RentalPipeline.Application.UseCases.Clients;

public class DeleteClientUseCase(
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork)
{
  public async Task ExecuteAsync(Guid clientId)
  {
    var client = await clientRepository.GetByIdAsync(clientId)
        ?? throw new KeyNotFoundException($"Cliente '{clientId}' não encontrado.");

    if (await clientRepository.HasPropertiesAsync(clientId))
      throw new InvalidOperationException(
          "Este cliente possui imóveis cadastrados e não pode ser removido.");

    if (await clientRepository.HasActiveProposalsAsync(clientId))
      throw new InvalidOperationException(
          "Este cliente possui propostas em andamento e não pode ser removido.");

    clientRepository.Remove(client);
    await unitOfWork.CommitAsync();
  }
}