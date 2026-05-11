namespace RentalPipeline.Application.DTOs.Responses;

public record AuthResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Token
);