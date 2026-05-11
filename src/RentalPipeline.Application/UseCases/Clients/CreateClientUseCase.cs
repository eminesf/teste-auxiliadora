using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.UseCases.Clients;

public class CreateClientUseCase(
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ClientResponse> ExecuteAsync(CreateClientRequest request)
    {
        if (await clientRepository.DocumentExistsAsync(request.Document))
            throw new InvalidOperationException(
                $"Já existe um cliente cadastrado com o documento '{request.Document}'.");

        var client = new Client(request.Name, request.Email, request.Document);
        await clientRepository.AddAsync(client);
        await unitOfWork.CommitAsync();
        return ClientResponse.FromEntity(client);
    }
}