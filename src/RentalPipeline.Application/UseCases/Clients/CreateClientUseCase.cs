using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Application.UseCases.Clients;

public class CreateClientUseCase(
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ClientResponse> ExecuteAsync(CreateClientRequest request)
    {
        if (await clientRepository.DocumentExistsAsync(
            new string(request.Document.Where(char.IsDigit).ToArray())))
            throw new InvalidOperationException(
                $"Já existe um cliente cadastrado com o documento '{request.Document}'.");

        var client = new Client(
            request.Name,
            request.Email,
            request.Document,
            Guid.NewGuid().ToString(),
            ClientRole.User
        );

        await clientRepository.AddAsync(client);
        await unitOfWork.CommitAsync();
        return ClientResponse.FromEntity(client);
    }
}