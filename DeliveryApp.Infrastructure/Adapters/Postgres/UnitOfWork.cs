using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

/// <summary>
/// Реализация UnitOfWork
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private bool _disposed;

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Сохранить изменения в рамках одной атомарной операции
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing) _dbContext.Dispose();
            _disposed = true;
        }
    }
}