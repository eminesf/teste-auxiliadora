using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.DTOs.Responses;

public record ProposalResponse(
    Guid Id,
    PropertyResponse Property,
    ClientResponse Client,
    int StatusCode,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static ProposalResponse FromEntity(Proposal p) => new(
        p.Id,
        PropertyResponse.FromEntity(p.Property),
        ClientResponse.FromEntity(p.Client),
        (int)p.Status,
        p.Status.ToString(),
        p.CreatedAt,
        p.UpdatedAt
    );
}