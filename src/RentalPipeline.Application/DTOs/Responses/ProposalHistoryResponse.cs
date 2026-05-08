using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.DTOs.Responses;

public record ProposalHistoryResponse(
    Guid Id,
    string? FromStatus,
    string ToStatus,
    DateTime OccurredAt
)
{
    public static ProposalHistoryResponse FromEntity(ProposalHistory h) => new(
        h.Id,
        h.FromStatus?.ToString(),  // null na criação — mostra null no JSON
        h.ToStatus.ToString(),
        h.OccurredAt
    );
}