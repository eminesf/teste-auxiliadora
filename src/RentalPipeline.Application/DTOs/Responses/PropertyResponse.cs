using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.DTOs.Responses;

public record PropertyResponse(
    Guid Id,
    ClientResponse Owner,
    string Street,
    string Number,
    string? Complement,
    string District,
    string City,
    string State,
    decimal RentPrice,
    int StatusCode,
    string Status,
    DateTime CreatedAt
)
{
    public static PropertyResponse FromEntity(Property p) => new(
        p.Id,
        ClientResponse.FromEntity(p.Owner),
        p.Street,
        p.Number,
        p.Complement,
        p.District,
        p.City,
        p.State,
        p.RentPrice,
        (int)p.Status,
        p.Status.ToString(),
        p.CreatedAt
    );
}