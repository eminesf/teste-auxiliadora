using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id);
    Task<Property?> GetByIdWithLockAsync(Guid id);
    Task<IEnumerable<Property>> GetAllAsync();
    Task AddAsync(Property property);
    Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);

}