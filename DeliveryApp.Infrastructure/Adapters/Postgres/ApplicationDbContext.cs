using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

/// <summary>
/// Контекст базы данных
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <inheritdoc />
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
        
    /// <summary>
    /// Заказы
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Курьеры
    /// </summary>
    public DbSet<Courier> Couriers { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}