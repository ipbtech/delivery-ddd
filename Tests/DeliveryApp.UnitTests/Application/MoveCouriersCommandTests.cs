using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
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
using Xunit;

namespace DeliveryApp.UnitTests.Application;
public class MoveCouriersCommandTests
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly ICourierRepository _courierRepositoryMock;
    private readonly IUnitOfWork _unitOfWork;

    public MoveCouriersCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _courierRepositoryMock = Substitute.For<ICourierRepository>();
    }

    private Maybe<Courier> SomeCourier()
    {
        return Courier.Create("prince", 3, Location.Min).Value;
    }

    private List<Order> AssignedOrders()
    {
        var orders = new List<Order>()
        {
            Order.Create(Guid.NewGuid(), Location.Max, 3).Value,
            Order.Create(Guid.NewGuid(), Location.CreateRandom().Value, 3).Value,
        };

        orders.ForEach(x => x.Assign(SomeCourier().Value));
        return orders;
    }

    [Fact]
    public async Task ReturnFalseWhenAllCouriersMovedSuccessfully()
    {
        //Arrange
        _orderRepositoryMock.GetAllInAssignedStatusAsync()
            .Returns(Task.FromResult(AssignedOrders()));
        _courierRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(SomeCourier()));

        //Act
        var moveCouriersCommand = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_unitOfWork, _orderRepositoryMock, _courierRepositoryMock);
        var result = await handler.Handle(moveCouriersCommand, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        _orderRepositoryMock.Received().Update(Arg.Any<Order>());
        _courierRepositoryMock.Received().Update(Arg.Any<Courier>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnFalseWhenNotOrdersWithAssignedStatus()
    {
        //Arrange
        _orderRepositoryMock.GetAllInAssignedStatusAsync()
            .Returns( Task.FromResult(new List<Order>()));

        //Act
        var moveCouriersCommand = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_unitOfWork, _orderRepositoryMock, _courierRepositoryMock);
        var result = await handler.Handle(moveCouriersCommand, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        await _orderRepositoryMock.Received(1).GetAllInAssignedStatusAsync();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}