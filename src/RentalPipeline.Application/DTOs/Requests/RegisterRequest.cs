namespace RentalPipeline.Application.DTOs.Requests;

public record RegisterRequest(
    string Name,
    string Email,
    string Document,
    string Password
);