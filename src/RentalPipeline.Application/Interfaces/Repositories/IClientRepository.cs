using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.Interfaces.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetAllAsync();
    Task AddAsync(Client client);
    Task<bool> DocumentExistsAsync(string document);
    Task<bool> HasPropertiesAsync(Guid clientId);
    Task<bool> HasActiveProposalsAsync(Guid clientId);
    void Remove(Client client);
}