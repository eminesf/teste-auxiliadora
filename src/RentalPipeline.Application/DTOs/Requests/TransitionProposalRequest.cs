namespace RentalPipeline.Application.DTOs.Requests;

public record TransitionProposalRequest(
    string NewStatus
);