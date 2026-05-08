namespace RentalPipeline.Application.DTOs.Requests;

public record CreateClientRequest(
    string Name,
    string Email,
    string Document
);