using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports.Repositories;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application;

public class CreateOrderCommandTests
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
    }

    private Maybe<Order> EmptyOrder()
    {
        return null;
    }

    private Maybe<Order> ExistedOrder()
    {
        return Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value,5).Value;
    }

    [Fact]
    public async Task ReturnTrueWhenOrderExists()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(ExistedOrder()));

        //Act
        var createCreateOrderCommandResult = CreateOrderCommand.Create(Guid.NewGuid(), "улица",5);
        createCreateOrderCommandResult.IsSuccess.Should().BeTrue();
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock);
        var result = await handler.Handle(createCreateOrderCommandResult.Value, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ReturnTrueWhenOrderCreatedSuccessfully()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(EmptyOrder()));

        //Act
        var createCreateOrderCommandResult = CreateOrderCommand.Create(Guid.NewGuid(), "улица",5);
        createCreateOrderCommandResult.IsSuccess.Should().BeTrue();
        var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock);
        var result = await handler.Handle(createCreateOrderCommandResult.Value, CancellationToken.None);

        //Assert
        await _orderRepositoryMock.Received(1).AddAsync(Arg.Any<Order>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        result.IsSuccess.Should().BeTrue();
    }
}