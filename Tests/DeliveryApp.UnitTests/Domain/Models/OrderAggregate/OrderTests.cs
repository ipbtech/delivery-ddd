using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.OrderAggregate;

public class OrderTests
{
    [Fact]
    public void CreateWithSuccessfulParameters()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var volume = 100;

        //Act
        var result = Order.Create(orderId, Location.Min, volume);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Location.Should().BeEquivalentTo(Location.Min);
        result.Value.Volume.Should().Be(volume);
        result.Value.Status.Should().Be(OrderStatus.Created);
    }

    [Fact]
    public void CreateWithEmptyGuid()
    {
        //Arrange

        //Act
        var result = Order.Create(default, Location.Min, 100);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void CreateWithInvalidVolume(int volume)
    {
        //Arrange
        var orderId = Guid.NewGuid();

        //Act
        var result = Order.Create(orderId, Location.Min, volume);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AssignSuccessful()
    {
        //Arrange
        var courierCreatingResult = Courier.Create("courier", 2, Location.Min);
        var orderCreatingResult = Order.Create(Guid.NewGuid(), Location.Min, 100);

        //Act
        var result = orderCreatingResult.Value.Assign(courierCreatingResult.Value);

        //Assert
        result.IsSuccess.Should().BeTrue();
        orderCreatingResult.Value.Status.Should().Be(OrderStatus.Assigned);
        orderCreatingResult.Value.CourierId.Should().NotBeNull();
    }

    [Fact]
    public void AssignWhenStatusNotCreated()
    {
        //Arrange
        var courierCreatingResult = Courier.Create("courier", 2, Location.Min);
        var orderCreatingResult = Order.Create(Guid.NewGuid(), Location.Min, 100);
        orderCreatingResult.Value.Assign(courierCreatingResult.Value);

        //Act
        var result = orderCreatingResult.Value.Assign(courierCreatingResult.Value);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CompleteSuccessful()
    {
        //Arrange
        var courierCreatingResult = Courier.Create("courier", 2, Location.Min);
        var orderCreatingResult = Order.Create(Guid.NewGuid(), Location.Min, 100);
        orderCreatingResult.Value.Assign(courierCreatingResult.Value);

        //Act
        var result = orderCreatingResult.Value.Complete();

        //Assert
        result.IsSuccess.Should().BeTrue();
        orderCreatingResult.Value.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void CompleteWhenNotAssigned()
    {
        //Arrange
        var orderCreatingResult = Order.Create(Guid.NewGuid(), Location.Min, 100);

        //Act
        var result = orderCreatingResult.Value.Complete();

        //Assert
        result.IsFailure.Should().BeTrue();
    }
}