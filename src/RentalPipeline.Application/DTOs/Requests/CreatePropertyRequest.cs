namespace RentalPipeline.Application.DTOs.Requests;

public record CreatePropertyRequest(
    Guid OwnerId,
    string Street,
    string Number,
    string District,
    string City,
    string State,
    decimal RentPrice,
    string? Complement = null
);