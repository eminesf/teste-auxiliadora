using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Application.UseCases.Proposals;

public class TransitionProposalUseCase(
    IProposalRepository proposalRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ProposalResponse> ExecuteAsync(Guid proposalId, TransitionProposalRequest request)
    {
        if (!Enum.TryParse<ProposalStatus>(request.NewStatus, ignoreCase: true, out var newStatus))
            throw new ArgumentException(
                $"Status inválido: '{request.NewStatus}'. " +
                $"Valores aceitos: {string.Join(", ", Enum.GetNames<ProposalStatus>())}");

        var proposal = await proposalRepository.GetByIdWithHistoryAsync(proposalId)
            ?? throw new KeyNotFoundException($"Proposta '{proposalId}' não encontrada.");

        // TransitionTo agora retorna o histórico — a entidade não gerencia mais a lista
        var history = proposal.TransitionTo(newStatus);

        // Salva o histórico diretamente no contexto
        await proposalRepository.AddHistoryAsync(history);

        await unitOfWork.CommitAsync();

        return ProposalResponse.FromEntity(proposal);
    }
}