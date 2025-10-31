using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports.Repositories;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.AssignOrder;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Services;
using Xunit;

namespace DeliveryApp.UnitTests.Application;

public class AssignOrderCommandTests
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly ICourierRepository _courierRepositoryMock;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOrderCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _courierRepositoryMock = Substitute.For<ICourierRepository>();
    }

    private Maybe<Order> SomeOrder()
    {
        return Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value, 5).Value;
    }

    private List<Courier> FreeCouriers()
    {
        return new List<Courier>()
        {
            Courier.Create("prince", 3, Location.Min).Value,
            Courier.Create("king", 4, Location.Max).Value
        };
    }

    [Fact]
    public async Task ReturnTrueWhenOrderAssignedSuccessfully()
    {
        //Arrange
        _orderRepositoryMock.GetFirstInCreatedStatusAsync()
            .Returns(Task.FromResult(SomeOrder()));
        _courierRepositoryMock.GetAllFreeAsync()
            .Returns(Task.FromResult(FreeCouriers()));

        //Act
        var assignOrderCommand = new AssignOrderCommand();
        var handler = new AssignOrderHandler(_unitOfWork, _orderRepositoryMock, _courierRepositoryMock, new DispatchService());
        var result = await handler.Handle(assignOrderCommand, CancellationToken.None);

        //Assert
        _orderRepositoryMock.Received(1).Update(Arg.Any<Order>());
        _courierRepositoryMock.Received(1).Update(Arg.Any<Courier>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ReturnFalseWhenNotAvailableOrders()
    {
        //Arrange
        _courierRepositoryMock.GetAllFreeAsync()
            .Returns(Task.FromResult(FreeCouriers()));

        //Act
        var assignOrderCommand = new AssignOrderCommand();
        var handler = new AssignOrderHandler(_unitOfWork, _orderRepositoryMock, _courierRepositoryMock, new DispatchService());
        var result = await handler.Handle(assignOrderCommand, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ReturnFalseWhenNotAvailableCouriers()
    {
        //Arrange
        _orderRepositoryMock.GetFirstInCreatedStatusAsync()
            .Returns(Task.FromResult(SomeOrder()));
        _courierRepositoryMock.GetAllFreeAsync()
            .Returns(Task.FromResult(new List<Courier>()));

        //Act
        var assignOrderCommand = new AssignOrderCommand();
        var handler = new AssignOrderHandler(_unitOfWork, _orderRepositoryMock, _courierRepositoryMock, new DispatchService());
        var result = await handler.Handle(assignOrderCommand, CancellationToken.None);

        //Assert
        result.IsFailure.Should().BeTrue();
    }
}