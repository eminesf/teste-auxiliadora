namespace RentalPipeline.Application.Events;

// Representa o contrato do evento publicado para o sistema financeiro
public record ContractActivatedEvent(
    Guid ProposalId,
    Guid PropertyId,
    Guid ClientId,
    DateTime ActivatedAt
);