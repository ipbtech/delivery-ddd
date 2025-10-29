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

public class CourierRepositoryShould : IAsyncLifetime
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
    public CourierRepositoryShould()
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
    public async Task CanAddCourier()
    {
        //Arrange
        var courierCreateResult = Courier.Create( "Pedestrian", 1, Location.Min);
        courierCreateResult.IsSuccess.Should().BeTrue();
        var courier = courierCreateResult.Value;

        //Act
        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getCourierResult = await courierRepository.GetAsync(courier.Id);
        getCourierResult.HasValue.Should().BeTrue();
        var courierFromDb = getCourierResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanUpdateCourier()
    {
        //Arrange
        var orderCreateResult = Order.Create(Guid.NewGuid(), Location.Min,5);
        orderCreateResult.IsSuccess.Should().BeTrue();
        var order = orderCreateResult.Value;

        var courierCreateResult = Courier.Create( "Pedestrian", 1, Location.Min);
        courierCreateResult.IsSuccess.Should().BeTrue();
        var courier = courierCreateResult.Value;

        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Act
        var courierStartWorkResult = courier.TakeOrder(order);
        courierStartWorkResult.IsSuccess.Should().BeTrue();
        courierRepository.Update(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getCourierResult = await courierRepository.GetAsync(courier.Id);
        getCourierResult.HasValue.Should().BeTrue();
        var courierFromDb = getCourierResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var courierCreateResult = Courier.Create("Pedestrian", 1, Location.Min);
        courierCreateResult.IsSuccess.Should().BeTrue();
        var courier = courierCreateResult.Value;

        //Act
        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var getCourierResult = await courierRepository.GetAsync(courier.Id);
        getCourierResult.HasValue.Should().BeTrue();
        var courierFromDb = getCourierResult.Value;
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanGetAllFree()
    {
        //Arrange
        var orderCreateResult = Order.Create(Guid.NewGuid(), Location.Min,5);
        orderCreateResult.IsSuccess.Should().BeTrue();
        var order = orderCreateResult.Value;
        
        var courier1CreateResult = Courier.Create("Pedestrian", 1, Location.Min);
        courier1CreateResult.IsSuccess.Should().BeTrue();
        var courier1 = courier1CreateResult.Value;
        var courierTakeOrderResult =courier1.TakeOrder(order);
        courierTakeOrderResult.IsSuccess.Should().BeTrue();

        var courier2CreateResult = Courier.Create( "Pedestrian",1, Location.Min);
        courier2CreateResult.IsSuccess.Should().BeTrue();
        var courier2 = courier2CreateResult.Value;

        var courierRepository = new CourierRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        await courierRepository.AddAsync(courier1);
        await courierRepository.AddAsync(courier2);
        await unitOfWork.SaveChangesAsync();

        //Act
        var activeCouriersFromDb = await courierRepository.GetAllFreeAsync();

        //Assert
        var couriersFromDb = activeCouriersFromDb.ToList();
        couriersFromDb.Should().NotBeEmpty();
        couriersFromDb.Count().Should().Be(1);
        couriersFromDb.First().Should().BeEquivalentTo(courier2);
    }
}