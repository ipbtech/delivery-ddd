using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Ports.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

/// <inheritdoc />
public class CourierRepository(ApplicationDbContext dbContext) : ICourierRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <inheritdoc />
    public async Task AddAsync(Courier courier)
    {
        await _dbContext.Couriers.AddAsync(courier);
    }

    /// <inheritdoc />
    public void Update(Courier courier)
    {
        _dbContext.Couriers.Update(courier);
    }

    /// <inheritdoc />
    public async Task<Maybe<Courier>> GetAsync(Guid courierId)
    {
        return await _dbContext.Couriers
            .SingleOrDefaultAsync(o => o.Id == courierId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Courier>> GetAllFreeAsync()
    {
        return await _dbContext.Couriers
            .Where(o => o.StoragePlaces.All(c=> !c.OrderId.HasValue)).ToListAsync();
    }
}