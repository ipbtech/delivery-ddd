using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using System.Linq;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.CourierAggregate;

public class CourierTests
{
    [Fact]
    public void CreateWithSuccessfulParameters()
    {
        //Arrange
        var name = "courier";
        var speed = 2;

        //Act
        var result = Courier.Create(name, speed, Location.Max);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Name.Should().BeEquivalentTo(name);
        result.Value.Speed.Should().Be(speed);
        result.Value.Location.Should().BeEquivalentTo(Location.Max);
        result.Value.StoragePlaces.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData(" ", 2)]
    [InlineData("1", 0)]
    [InlineData("1", -10)]
    public void CreateWithUnsuccessfulParameters(string name, int speed)
    {
        //Arrange

        //Act
        var result = Courier.Create(name, speed, Location.Max);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddStoragePlaceWithSuccessfulParameters()
    {
        //Arrange
        var storagePlaceName = "place";
        var courierResult = Courier.Create("courier", 2, Location.Max);

        //Act
        var result = courierResult.Value.AddStoragePlace(storagePlaceName, 10);

        //Assert
        result.IsSuccess.Should().BeTrue();
        courierResult.Value.StoragePlaces.FirstOrDefault(x => x.Name == storagePlaceName).Should().NotBeNull();
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData(" ", 2)]
    [InlineData("1", 0)]
    [InlineData("1", -10)]
    public void AddStoragePlaceWithUnsuccessfulParameters(string name, int volume)
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Max);

        //Act
        var result = courierResult.Value.AddStoragePlace(name, volume);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TakeOrderSuccessful()
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Max);
        var orderId = Guid.NewGuid();
        var orderResult = Order.Create(orderId, Location.Min, 3);

        //Act
        var result = courierResult.Value.TakeOrder(orderResult.Value);

        //Assert
        result.IsSuccess.Should().BeTrue();
        courierResult.Value.StoragePlaces.FirstOrDefault(x => x.OrderId == orderId).Should().NotBeNull();
    }

    [Fact]
    public void TakeOrderWhenCourierOccupied()
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Max);
        var orderId = Guid.NewGuid();
        var orderResult = Order.Create(orderId, Location.Min, 11);

        //Act
        var result = courierResult.Value.TakeOrder(orderResult.Value);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CanTakeOrderSuccessful()
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Max);
        var orderResult = Order.Create(Guid.NewGuid(), Location.Min, 3);

        //Act
        var result = courierResult.Value.CanTakeOrder(orderResult.Value);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public void CanTakeOrderWhenCourierOccupied()
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Max);
        var orderResult = Order.Create(Guid.NewGuid(), Location.Min, 11);

        //Act
        var result = courierResult.Value.CanTakeOrder(orderResult.Value);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public void CompleteOrderSuccessful()
    {
        //Arrange
        var orderResult = Order.Create(Guid.NewGuid(), Location.Min, 5);
        var courierResult = Courier.Create("courier", 2, Location.Max);
        courierResult.Value.TakeOrder(orderResult.Value);

        //Act
        var result = courierResult.Value.CompleteOrder(orderResult.Value);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CompleteOrderWhenOrderNotFoundIntoStoragePlaces()
    {
        //Arrange
        var orderResult = Order.Create(Guid.NewGuid(), Location.Min, 5);
        var courierResult = Courier.Create("courier", 2, Location.Max);
        //courierResult.Value.TakeOrder(orderResult.Value);

        //Act
        var result = courierResult.Value.CompleteOrder(orderResult.Value);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CalculateTimeToLocationSuccessful()
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Min);

        //Act
        var result = courierResult.Value.CalculateTimeToLocation(Location.Max);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MoveCourierSuccessful()
    {
        //Arrange
        var courierResult = Courier.Create("courier", 2, Location.Min);

        //Act
        var result = courierResult.Value.Move(Location.Max);

        //Assert
        result.IsSuccess.Should().BeTrue();
        courierResult.Value.Location.Should().NotBeEquivalentTo(Location.Min);
    }
}