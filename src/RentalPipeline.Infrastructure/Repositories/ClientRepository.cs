using Microsoft.EntityFrameworkCore;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Enums;
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
    public async Task<bool> DocumentExistsAsync(string document) =>
        await context.Clients.AnyAsync(c => c.Document == document);

    public async Task<bool> HasPropertiesAsync(Guid clientId) =>
        await context.Properties.AnyAsync(p => p.OwnerId == clientId);

    public async Task<bool> HasActiveProposalsAsync(Guid clientId) =>
        await context.Proposals.AnyAsync(p =>
            p.ClientId == clientId &&
            p.Status != ProposalStatus.Ativo &&
            p.Status != ProposalStatus.Reprovada &&
            p.Status != ProposalStatus.Cancelada);

    public void Remove(Client client) =>
        context.Clients.Remove(client);
}