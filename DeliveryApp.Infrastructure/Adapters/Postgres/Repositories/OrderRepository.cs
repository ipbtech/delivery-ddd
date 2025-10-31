using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

/// <inheritdoc />
public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <inheritdoc />
    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    /// <inheritdoc />
    public void Update(Order order)
    {
        _dbContext.Orders.Update(order);
    }

    /// <inheritdoc />
    public async Task<Maybe<Order>> GetAsync(Guid orderId)
    {
        return await _dbContext.Orders
            .SingleOrDefaultAsync(o => o.Id == orderId);
    }

    /// <inheritdoc />
    public async Task<Maybe<Order>> GetFirstInCreatedStatusAsync()
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Status.Name == OrderStatus.Created.Name);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Order>> GetAllInAssignedStatusAsync()
    {
        return await _dbContext.Orders
            .Where(o => o.Status.Name == OrderStatus.Assigned.Name).ToListAsync();
    }
}