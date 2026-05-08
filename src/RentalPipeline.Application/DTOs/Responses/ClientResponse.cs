using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.DTOs.Responses;

public record ClientResponse(
    Guid Id,
    string Name,
    string Email,
    string Document,
    DateTime CreatedAt
)
{
    public static ClientResponse FromEntity(Client c) => new(
        c.Id,
        c.Name,
        c.Email,
        c.Document,
        c.CreatedAt
    );
}