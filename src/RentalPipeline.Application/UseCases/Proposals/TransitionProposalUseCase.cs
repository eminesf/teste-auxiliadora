using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Application.DTOs.Responses;
using RentalPipeline.Application.Events;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Application.UseCases.Proposals;

public class TransitionProposalUseCase(
    IProposalRepository proposalRepository,
    IEventPublisher eventPublisher,
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

        var history = proposal.TransitionTo(newStatus);
        await proposalRepository.AddHistoryAsync(history);
        await unitOfWork.CommitAsync();

        // Dispara o evento APÓS o commit — garante que só publica se salvou com sucesso
        if (newStatus == ProposalStatus.Ativo)
        {
            await eventPublisher.PublishAsync(new ContractActivatedEvent(
                ProposalId: proposal.Id,
                PropertyId: proposal.PropertyId,
                ClientId: proposal.ClientId,
                ActivatedAt: DateTime.UtcNow
            ));
        }

        return ProposalResponse.FromEntity(proposal);
    }
}