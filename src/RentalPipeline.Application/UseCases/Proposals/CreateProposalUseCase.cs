using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Exceptions;

namespace RentalPipeline.Application.UseCases.Proposals;

public class CreateProposalUseCase(
    IProposalRepository proposalRepository,
    IPropertyRepository propertyRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ProposalResponse> ExecuteAsync(CreateProposalRequest request)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var property = await propertyRepository.GetByIdWithLockAsync(request.PropertyId)
                ?? throw new KeyNotFoundException($"Imóvel '{request.PropertyId}' não encontrado.");

            if (property.Status != Domain.Enums.PropertyStatus.Available)
                throw new PropertyNotAvailableException(request.PropertyId);

            // Owner não pode alugar o próprio imóvel
            if (property.OwnerId == request.ClientId)
                throw new ArgumentException("O proprietário não pode criar uma proposta para o próprio imóvel.");

            var client = await clientRepository.GetByIdAsync(request.ClientId)
                ?? throw new KeyNotFoundException($"Cliente '{request.ClientId}' não encontrado.");

            property.Status = Domain.Enums.PropertyStatus.UnderNegotiation;

            var proposal = new Proposal(request.PropertyId, request.ClientId);
            await proposalRepository.AddAsync(proposal);

            await unitOfWork.CommitAsync();
            await unitOfWork.CommitTransactionAsync();

            return ProposalResponse.FromEntity(proposal);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}