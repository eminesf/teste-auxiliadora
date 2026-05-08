using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;

namespace RentalPipeline.Application.UseCases.Proposals;

public class GetProposalHistoryUseCase(IProposalRepository proposalRepository)
{
    public async Task<IEnumerable<ProposalHistoryResponse>> ExecuteAsync(Guid proposalId)
    {
        var proposal = await proposalRepository.GetByIdWithHistoryAsync(proposalId)
            ?? throw new KeyNotFoundException($"Proposta '{proposalId}' não encontrada.");

        var history = await proposalRepository.GetHistoryAsync(proposalId);

        return history
            .OrderBy(h => h.OccurredAt)
            .Select(ProposalHistoryResponse.FromEntity);
    }
}