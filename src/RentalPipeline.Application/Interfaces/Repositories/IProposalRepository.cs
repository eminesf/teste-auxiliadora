using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Application.Interfaces.Repositories;

public interface IProposalRepository
{
    Task<Proposal?> GetByIdAsync(Guid id);
    Task<Proposal?> GetByIdWithHistoryAsync(Guid id);
    Task<IEnumerable<Proposal>> GetAllAsync();
    Task AddAsync(Proposal proposal);
    Task AddHistoryAsync(ProposalHistory history);
    Task<IEnumerable<ProposalHistory>> GetHistoryAsync(Guid proposalId);
}