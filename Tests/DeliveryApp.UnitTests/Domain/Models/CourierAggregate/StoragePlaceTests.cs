using DeliveryApp.Core.Domain.Models.CourierAggregate;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.CourierAggregate;

public class StoragePlaceTests
{
    [Fact]
    public void CreateWithSuccessfulParameters()
    {
        //Arrange
        var name = "place";
        var volume = 999;

        //Act
        var storageCreatingResult = StoragePlace.Create(name, volume);

        //Assert
        storageCreatingResult.IsSuccess.Should().BeTrue();
        storageCreatingResult.Value.Name.Should().Be(name);
        storageCreatingResult.Value.TotalVolume.Should().Be(volume);
        storageCreatingResult.Value.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData("", -5)]
    [InlineData(" ", 11)]
    public void CreateWithUnsuccessfulParameters(string name, int volume)
    {
        //Arrange

        //Act
        var storageCreatingResult = StoragePlace.Create(name,  volume);

        //Assert
        storageCreatingResult.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CanStoreSuccessful()
    {
        //Arrange
        var storageCreatingResult = StoragePlace.Create("place", 999);

        //Act
        var result = storageCreatingResult.Value.CanStore(888);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("1", 11, 0)]
    [InlineData("22", 10, 12)]
    public void CanStoreWithInvalidOrderVolume(string name, int storageVolume, int orderVolume)
    {
        //Arrange
        var storageCreatingResult = StoragePlace.Create(name, storageVolume);

        //Act
        var result = storageCreatingResult.Value.CanStore(orderVolume);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(999, 999)]
    [InlineData(999, 888)]
    public void StoreOrderSuccessful(int storageVolume, int orderVolume)
    {
        //Arrange
        var storageCreatingResult = StoragePlace.Create("place", storageVolume);
        var orderId = Guid.NewGuid();

        //Act
        storageCreatingResult.Value.StoreOrder(orderId, orderVolume);

        //Assert
        storageCreatingResult.IsSuccess.Should().BeTrue();
        storageCreatingResult.Value.IsOccupied.Should().BeTrue();
        storageCreatingResult.Value.OrderId.Should().Be(orderId);
    }

    [Theory]
    [InlineData(999, 1000)]
    [InlineData(999, 0)]
    public void StoreWhenOrderVolumeGreaterThanStorageVolume(int storageVolume, int orderVolume)
    {
        //Arrange
        var storageCreatingResult = StoragePlace.Create("place", storageVolume);
        var orderId = Guid.NewGuid();

        //Act
        var result = storageCreatingResult.Value.StoreOrder(orderId, orderVolume);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void StoreWithOccupiedStorage()
    {
        //Arrange
        var storageCreatingResult = StoragePlace.Create("place", 999);
        storageCreatingResult.Value.StoreOrder(Guid.NewGuid(), 999);

        //Act
        var result = storageCreatingResult.Value.StoreOrder(Guid.NewGuid(), 999);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ClearStorageSuccessful()
    {
        //Arrange
        var storageCreatingResult = StoragePlace.Create("place", 999);
        storageCreatingResult.Value.StoreOrder(Guid.NewGuid(), 999);

        //Act
        storageCreatingResult.Value.ClearOrder();

        //Assert
        storageCreatingResult.Value.IsOccupied.Should().BeFalse();
        storageCreatingResult.Value.OrderId.HasValue.Should().BeFalse();
    }
}