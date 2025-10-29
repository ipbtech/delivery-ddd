using System;
using System.Collections.Generic;
using System.Linq;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceTests
{
    [Fact]
    public void DispatchSuccessful()
    {
        //Arrange
        var assertedCourierName = "courier1";
        var couriers = new List<Courier>()
        {
            Courier.Create(assertedCourierName, 2, Location.Max).Value,
            Courier.Create("assertedCourierName", 2, Location.Create(2, 4).Value).Value,
            Courier.Create("CourierName", 2, Location.Min).Value,
        };

        var order = Order.Create(Guid.NewGuid(), Location.Create(9, 9).Value, 9).Value;

        //Act
        var service = new DispatchService();
        var result = service.Dispatch(couriers, order);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(assertedCourierName);
        order.Status.Should().Be(OrderStatus.Assigned);
    }

    [Fact]
    public void DispatchWhenOrderStatusNotCreated()
    {
        //Arrange
        var couriers = new List<Courier>()
        {
            Courier.Create("эээ", 2, Location.Max).Value,
            Courier.Create("assertedCourierName", 2, Location.Create(2, 4).Value).Value,
            Courier.Create("CourierName", 2, Location.Min).Value,
        };

        var order = Order.Create(Guid.NewGuid(), Location.Create(9, 9).Value, 9).Value;
        order.Assign(couriers.First());

        //Act
        var service = new DispatchService();
        var result = service.Dispatch(couriers, order);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void DispatchWhenAllCouriersOccupied()
    {
        //Arrange
        var couriers = new List<Courier>()
        {
            Courier.Create("эээ", 2, Location.Max).Value,
            Courier.Create("assertedCourierName", 2, Location.Create(2, 4).Value).Value,
            Courier.Create("CourierName", 2, Location.Min).Value,
        };

        var orders = new List<Order>()
        {
            Order.Create(Guid.NewGuid(), Location.Create(8, 7).Value, 9).Value,
            Order.Create(Guid.NewGuid(), Location.Create(1, 6).Value, 9).Value,
            Order.Create(Guid.NewGuid(), Location.Create(9, 9).Value, 9).Value,
        };

        var newOrder = Order.Create(Guid.NewGuid(), Location.Create(8, 7).Value, 9).Value;

        //Act
        var service = new DispatchService();

        //занимаем всех трех курьеров
        var dispatchResult1 = service.Dispatch(couriers, orders[0]);
        var dispatchResult2 = service.Dispatch(couriers, orders[1]);
        var dispatchResult3 = service.Dispatch(couriers, orders[2]);

        var result = service.Dispatch(couriers, newOrder);

        //Assert
        result.IsFailure.Should().BeTrue();
    }
}