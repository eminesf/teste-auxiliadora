using Microsoft.EntityFrameworkCore;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Infrastructure.Data;

namespace RentalPipeline.Infrastructure.Repositories;

public class ProposalRepository(AppDbContext context) : IProposalRepository
{
    public async Task<Proposal?> GetByIdAsync(Guid id) =>
        await context.Proposals
            .Include(p => p.Property).ThenInclude(p => p.Owner)
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Proposal?> GetByIdWithHistoryAsync(Guid id) =>
        await context.Proposals
            .Include(p => p.Property).ThenInclude(p => p.Owner)
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Proposal>> GetAllAsync() =>
        await context.Proposals
            .Include(p => p.Property).ThenInclude(p => p.Owner)
            .Include(p => p.Client)
            .ToListAsync();

    public async Task AddAsync(Proposal proposal) =>
        await context.Proposals.AddAsync(proposal);

    public async Task AddHistoryAsync(ProposalHistory history) =>
        await context.ProposalHistories.AddAsync(history);

    public async Task<IEnumerable<ProposalHistory>> GetHistoryAsync(Guid proposalId) =>
        await context.ProposalHistories
            .Where(h => h.ProposalId == proposalId)
            .OrderBy(h => h.OccurredAt)
            .ToListAsync();
}