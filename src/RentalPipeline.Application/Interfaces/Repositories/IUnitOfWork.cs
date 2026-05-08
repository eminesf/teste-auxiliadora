namespace RentalPipeline.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}