using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.DTOs.Responses;

public record ClientPublicResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt
)
{
  public static ClientPublicResponse FromEntity(Client c) => new(
      c.Id,
      c.Name,
      c.Email,
      c.CreatedAt
  );
}