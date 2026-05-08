using Microsoft.EntityFrameworkCore;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Infrastructure.Data;

namespace RentalPipeline.Infrastructure.Repositories;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    public async Task<Client?> GetByIdAsync(Guid id) =>
        await context.Clients.FindAsync(id);

    public async Task<IEnumerable<Client>> GetAllAsync() =>
        await context.Clients.ToListAsync();

    public async Task AddAsync(Client client) =>
        await context.Clients.AddAsync(client);
}