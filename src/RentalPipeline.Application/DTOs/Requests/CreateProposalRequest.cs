namespace RentalPipeline.Application.DTOs.Requests;

public record CreateProposalRequest(
    Guid PropertyId,
    Guid ClientId
);