using RentalPipeline.Domain.Enums;
using RentalPipeline.Domain.Exceptions;

namespace RentalPipeline.Domain.Entities;

public class Proposal
{
    private static readonly IReadOnlyDictionary<ProposalStatus, IReadOnlyList<ProposalStatus>> AllowedTransitions =
        new Dictionary<ProposalStatus, IReadOnlyList<ProposalStatus>>
        {
            { ProposalStatus.Nova,            [ProposalStatus.AnaliseCredito, ProposalStatus.Reprovada, ProposalStatus.Cancelada] },
            { ProposalStatus.AnaliseCredito,  [ProposalStatus.ContratoEmitido, ProposalStatus.Reprovada, ProposalStatus.Cancelada] },
            { ProposalStatus.ContratoEmitido, [ProposalStatus.Assinado, ProposalStatus.Reprovada, ProposalStatus.Cancelada] },
            { ProposalStatus.Assinado,        [ProposalStatus.Ativo, ProposalStatus.Cancelada] },
        };

    public Guid Id { get; private set; }
    public Client Client { get; private set; } = null!;
    public Guid PropertyId { get; private set; }
    public Guid ClientId { get; private set; }
    public ProposalStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Property Property { get; private set; } = null!;

    protected Proposal() { }

    public Proposal(Guid propertyId, Guid clientId)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ClientId = clientId;
        Status = ProposalStatus.Nova;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Retorna o histórico criado — quem salva é o use case
    public ProposalHistory TransitionTo(ProposalStatus newStatus)
    {
        if (!AllowedTransitions.TryGetValue(Status, out var allowed))
            throw new InvalidTransitionException(Status.ToString(), newStatus.ToString(), []);

        if (!allowed.Contains(newStatus))
            throw new InvalidTransitionException(
                Status.ToString(),
                newStatus.ToString(),
                allowed.Select(s => s.ToString()));

        var previousStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == ProposalStatus.Ativo)
            Property.Status = PropertyStatus.Rented;

        if (newStatus is ProposalStatus.Reprovada or ProposalStatus.Cancelada)
            Property.Status = PropertyStatus.Available;

        return new ProposalHistory(Id, previousStatus, newStatus);
    }
}