using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

/// <summary>
/// Location Value Object
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Location : ValueObject
{
    private const int _minCoordinate = 1;
    private const int _maxCoordinate = 10;

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Coordinate X
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Coordinate Y
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Available minimum location
    /// </summary>
    public static Location Min => new Location(_minCoordinate, _minCoordinate);

    /// <summary>
    /// Available maximum location
    /// </summary>
    public static Location Max => new Location(_maxCoordinate, _maxCoordinate);


    /// <summary>
    /// Create new location with passed coordinates
    /// </summary>
    /// <param name="x">Coordinate X</param>
    /// <param name="y">Coordinate Y</param>
    /// <returns>New Location</returns>
    public static Result<Location, Error> Create(int x, int y)
    {
        if (x < 1 || x > 10)
        {
            return GeneralErrors.ValueIsInvalid(nameof(x));
        }

        if (y < 1 || y > 10)
        {
            return GeneralErrors.ValueIsInvalid(nameof(y));
        }

        return new Location(x, y);
    }

    /// <summary>
    /// Create new random location
    /// </summary>
    /// <returns></returns>
    public static Result<Location> CreateRandom()
    {
        var random = new Random();

        return new Location(
            random.Next(_minCoordinate, _maxCoordinate),
            random.Next(_minCoordinate, _maxCoordinate)
            );
    }

    /// <summary>
    /// Calculate distance between two locations
    /// </summary>
    /// <param name="another">Another location</param>
    /// <returns>Distance between locations</returns>
    public Result<int, Error> DistanceBetween(Location another)
    {
        if (this == another)
        {
            return 0;
        }
        
        var xDistance = X - another.X;
        var yDistance = Y - another.Y;

        var result = Math.Abs(xDistance) + Math.Abs(yDistance);

        return result;
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}