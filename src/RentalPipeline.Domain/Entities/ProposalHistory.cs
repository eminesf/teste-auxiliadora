using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Domain.Entities;

public class ProposalHistory
{
    public Guid Id { get; private set; }
    public Guid ProposalId { get; private set; }
    public ProposalStatus? FromStatus { get; private set; } 
    public ProposalStatus ToStatus { get; private set; }
    public DateTime OccurredAt { get; private set; }

    protected ProposalHistory() { }

    public ProposalHistory(Guid proposalId, ProposalStatus? from, ProposalStatus to)
    {
        Id = Guid.NewGuid();
        ProposalId = proposalId;
        FromStatus = from;
        ToStatus = to;
        OccurredAt = DateTime.UtcNow;
    }
}