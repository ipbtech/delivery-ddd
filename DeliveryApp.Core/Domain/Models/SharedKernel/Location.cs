using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

/// <summary>
/// Локация
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Location : ValueObject
{
    private const int _minCoordinate = 1;
    private const int _maxCoordinate = 10;

    private static Lazy<Random> _lazyRandom = new();

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// X координата
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Y координата
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Допустимая минимальная локация
    /// </summary>
    public static Location Min => new(_minCoordinate, _minCoordinate);

    /// <summary>
    /// Допустимая максимальная локация
    /// </summary>
    public static Location Max => new(_maxCoordinate, _maxCoordinate);

    /// <summary>
    /// Создать новую локацию с переданными координатами
    /// </summary>
    /// <param name="x">X координата</param>
    /// <param name="y">Y координата</param>
    /// <returns>Новая локация</returns>
    public static Result<Location, Error> Create(int x, int y)
    {
        if (x < _minCoordinate || x > _maxCoordinate)
        {
            return GeneralErrors.ValueIsInvalid(nameof(x));
        }

        if (y < _minCoordinate || y > _maxCoordinate)
        {
            return GeneralErrors.ValueIsInvalid(nameof(y));
        }

        return new Location(x, y);
    }

    /// <summary>
    /// Создать новую локацию с рандомными координатами
    /// </summary>
    /// <returns>Новая локация</returns>
    public static Result<Location> CreateRandom()
    {
        return new Location(
            _lazyRandom.Value.Next(_minCoordinate, _maxCoordinate),
            _lazyRandom.Value.Next(_minCoordinate, _maxCoordinate)
            );
    }

    /// <summary>
    /// Рассчитать расстояние между двумя локациями как сумму длин между X и Y
    /// </summary>
    /// <param name="another">Другая локация</param>
    /// <returns>Расстояние между локациями</returns>
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