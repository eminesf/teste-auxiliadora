using Microsoft.EntityFrameworkCore;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Proposal> Proposals => Set<Proposal>();
    public DbSet<ProposalHistory> ProposalHistories => Set<ProposalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todos os arquivos de mapping desta pasta automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}