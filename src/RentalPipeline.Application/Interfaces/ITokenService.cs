using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.Interfaces;

public interface ITokenService
{
  string GenerateToken(Client client);
}