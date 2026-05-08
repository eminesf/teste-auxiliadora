using Microsoft.EntityFrameworkCore.Storage;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Infrastructure.Data;

namespace RentalPipeline.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task<int> CommitAsync() =>
        await context.SaveChangesAsync();

    public async Task BeginTransactionAsync() =>
        _transaction = await context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction is null) return;
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose() => _transaction?.Dispose();
}