using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.UseCases.Properties;

public class CreatePropertyUseCase(
    IPropertyRepository propertyRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<PropertyResponse> ExecuteAsync(CreatePropertyRequest request)
    {
        var owner = await clientRepository.GetByIdAsync(request.OwnerId)
            ?? throw new KeyNotFoundException($"Owner '{request.OwnerId}' não encontrado.");

        var property = new Property(
            request.OwnerId,
            request.Street,
            request.Number,
            request.District,
            request.City,
            request.State,
            request.RentPrice,
            request.Complement
        );

        await propertyRepository.AddAsync(property);
        await unitOfWork.CommitAsync();

        // Recarrega com Owner para montar o response
        var created = await propertyRepository.GetByIdAsync(property.Id)
            ?? throw new InvalidOperationException("Erro ao recarregar imóvel.");

        return PropertyResponse.FromEntity(created);
    }
}