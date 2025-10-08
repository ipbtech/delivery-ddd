using System.Diagnostics.CodeAnalysis;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel;

public class LocationTests
{
    [Fact]
    public void SuccessCreatedWithRandom()
    {
        //Arrange

        //Act
        var location = Location.CreateRandom();

        //Assert
        location.IsSuccess.Should().BeTrue();
        location.Value.X.Should().BeLessThanOrEqualTo(10);
        location.Value.Y.Should().BeLessThanOrEqualTo(10);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 7)]
    [InlineData(8, 5)]
    [InlineData(10, 1)]
    [InlineData(10,10)]
    public void SuccessCreatedWithParameters(int x, int y)
    {
        //Arrange

        //Act
        var location = Location.Create(x, y);

        //Assert
        location.IsSuccess.Should().BeTrue();
        location.Value.X.Should().Be(x);
        location.Value.Y.Should().Be(y);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 0)]
    [InlineData(11, 5)]
    [InlineData(21, 33)]
    [InlineData(10, 0)]
    public void FailedCreatedWithInvalidParameters(int x, int y)
    {
        //Arrange

        //Act
        var location = Location.Create(x, y);

        //Assert
        location.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void SuccessCalculateDistanceBetween()
    {
        //Arrange
        var max = Location.Max;
        var min = Location.Min;

        //Act
        var distance = min.DistanceBetween(max);

        //Assert
        distance.IsSuccess.Should().BeTrue();
        distance.Value.Should().Be(18);
    }

    [Fact]
    [SuppressMessage("ReSharper", "UsageOfDefaultStructEquality")]
    public void CheckEquality()
    {
        //Arrange
        var left = Location.Create(7,7);
        var right = Location.Create(7, 7);

        //Act
        var result = right.Equals(left);

        //Assert
        result.Should().BeTrue();
    }
}