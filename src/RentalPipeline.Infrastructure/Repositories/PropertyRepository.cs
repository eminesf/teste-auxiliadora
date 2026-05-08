using Microsoft.EntityFrameworkCore;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Infrastructure.Data;

namespace RentalPipeline.Infrastructure.Repositories;

public class PropertyRepository(AppDbContext context) : IPropertyRepository
{
    public async Task<Property?> GetByIdAsync(Guid id) =>
        await context.Properties
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Property>> GetAllAsync() =>
        await context.Properties
            .Include(p => p.Owner)
            .ToListAsync();

    public async Task AddAsync(Property property) =>
        await context.Properties.AddAsync(property);

    public async Task<Property?> GetByIdWithLockAsync(Guid id) =>
        await context.Properties
            .FromSqlRaw("SELECT * FROM properties WHERE id = {0} FOR UPDATE", id)
            .Include(p => p.Owner)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId) =>
        await context.Properties
            .Include(p => p.Owner)
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync();
}