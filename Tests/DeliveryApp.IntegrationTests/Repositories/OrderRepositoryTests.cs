using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
{
    /// <summary>
    ///     Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("order")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private ApplicationDbContext _context;

    /// <summary>
    ///     Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public OrderRepositoryShould()
    { }

    /// <summary>
    ///     Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
        await _postgreSqlContainer.StartAsync();

        //Накатываем миграции и справочники
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); })
            .Options;
        _context = new ApplicationDbContext(contextOptions);
        await _context.Database.MigrateAsync();
    }

    /// <summary>
    ///     Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CanAddOrder()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, Location.Min,5).Value;

        //Act
        var orderRepository = new OrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getOrderResult = await orderRepository.GetAsync(order.Id);
        getOrderResult.HasValue.Should().BeTrue();
        var orderFromDb = getOrderResult.Value;
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanUpdateOrder()
    {
        //Arrange
        var courier = Courier.Create( "Pedestrian", 1, Location.Create(1, 1).Value).Value;

        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, Location.Min,5).Value;

        var orderRepository = new OrderRepository(_context);
        await orderRepository.AddAsync(order);

        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Act
        var orderAssignToCourierResult = order.Assign(courier);
        orderAssignToCourierResult.IsSuccess.Should().BeTrue();
        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getOrderResult = await orderRepository.GetAsync(order.Id);
        getOrderResult.HasValue.Should().BeTrue();
        var orderFromDb = getOrderResult.Value;
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, Location.Min,5).Value;

        //Act
        var orderRepository = new OrderRepository(_context);
        await orderRepository.AddAsync(order);

        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getOrderResult = await orderRepository.GetAsync(order.Id);
        getOrderResult.HasValue.Should().BeTrue();
        var orderFromDb = getOrderResult.Value;
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanGetFirstInCreatedStatus()
    {
        //Arrange
        var courier = Courier.Create( "Pedestrian", 1, Location.Create(1, 1).Value).Value;

        var order1Id = Guid.NewGuid();
        var order1 = Order.Create(order1Id, Location.Min,5).Value;
        var orderAssignToCourierResult = order1.Assign(courier);
        orderAssignToCourierResult.IsSuccess.Should().BeTrue();

        var order2Id = Guid.NewGuid();
        var order2 = Order.Create(order2Id, Location.Min,5).Value;

        var orderRepository = new OrderRepository(_context);
        await orderRepository.AddAsync(order1);
        await orderRepository.AddAsync(order2);

        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Act
        var getFirstOrderInCreatedStatusFromDbResult = await orderRepository.GetFirstInCreatedStatusAsync();

        //Assert
        getFirstOrderInCreatedStatusFromDbResult.HasValue.Should().BeTrue();
        var orderFromDb = getFirstOrderInCreatedStatusFromDbResult.Value;
        order2.Should().BeEquivalentTo(orderFromDb);
    }
    
    [Fact]
    public async Task WhenSaveOrdersWithSameLocationThenShouldCorrectSavingAtDatabase()
    {
        // Arrange
        var firstOrderId = Guid.NewGuid();
        var secondOrderId = Guid.NewGuid();

        var firstOrder = Order.Create(firstOrderId, Location.Min,5).Value;
        var secondOrder = Order.Create(secondOrderId, Location.Min,5).Value;
        var orderRepository = new OrderRepository(_context);
        var unitOfWork = new UnitOfWork(_context);

        await orderRepository.AddAsync(firstOrder);
        await orderRepository.AddAsync(secondOrder);
        await unitOfWork.SaveChangesAsync();

        // Act
        var first = await orderRepository.GetAsync(firstOrderId);
        var second = await orderRepository.GetAsync(secondOrderId);

        // Assert
        first.Value.Location.Should().Be(Location.Min);
        second.Value.Location.Should().Be(Location.Min);
    }
}