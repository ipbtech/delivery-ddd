using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

/// <summary>
/// Курьер
/// </summary>
public sealed class Courier : Aggregate<Guid>
{
    // ctr for EF Core
    private Courier()
    { }

    private Courier(string name, int speed, Location location, StoragePlace storagePlace) : this()
    {
        Id = Guid.NewGuid();
        Name = name;
        Speed = speed;
        Location = location;
        StoragePlaces = new() { storagePlace };
    }

    /// <summary>
    /// Имя курьера
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Скорость курьера
    /// </summary>
    public int Speed { get; }

    /// <summary>
    /// Местоположение курьера
    /// </summary>
    public Location Location { get; private set; }

    /// <summary>
    /// Места хранения заказов у курьера
    /// </summary>
    public List<StoragePlace> StoragePlaces { get; }

    /// <summary>
    /// Создать курьера
    /// </summary>
    /// <param name="name">Имя курьера</param>
    /// <param name="speed">Скорость курьера</param>
    /// <param name="location">Местоположение курьера</param>
    /// <returns>Новый курьер</returns>
    public static Result<Courier, Error> Create(string name, int speed, Location location)
    {
        var validateResult = Validate(name, speed);
        if (validateResult.IsSuccess)
        {
            var defaultStoragePlace = StoragePlace.Create("Сумка", 10).Value;
            return new Courier(name, speed, location, defaultStoragePlace);
        }

        return validateResult.Error;
    }

    /// <summary>
    /// Добавить место хранения
    /// </summary>
    /// <param name="name">Имя места хранения</param>
    /// <param name="volume">Объем места хранения</param>
    public UnitResult<Error> AddStoragePlace(string name, int volume)
    {
        var validateResult = Validate(name, volume);
        if (validateResult.IsSuccess)
        {
            var newStoragePlace = StoragePlace.Create(name, volume).Value;
            StoragePlaces.Add(newStoragePlace);
            return UnitResult.Success<Error>();
        }

        return validateResult.Error;
    }

    /// <summary>
    /// Проверить может ли курьер взять заказ
    /// </summary>
    /// <param name="order">Заказ</param>
    /// <returns>True - если может взять заказ, False - если все места хранения заняты</returns>
    public Result<bool, Error> CanTakeOrder(Order order)
    {
        foreach (var storagePlace in StoragePlaces)
        {
            var result = storagePlace.CanStore(order.Volume);
            if (result.IsSuccess)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Взять заказ
    /// </summary>
    /// <param name="order">Заказ</param>
    public UnitResult<Error> TakeOrder(Order order)
    {
        foreach (var storagePlace in StoragePlaces)
        {
            var result = storagePlace.CanStore(order.Volume);
            if (result.IsSuccess)
            {
                return storagePlace.StoreOrder(order.Id, order.Volume);
            }
        }

        return Errors.CourierOccupied();
    }

    /// <summary>
    /// Завершить заказ
    /// </summary>
    /// <param name="order">Заказ</param>
    public UnitResult<Error> CompleteOrder(Order order)
    {
        var storagePlace = StoragePlaces.FirstOrDefault(x => x.OrderId == order.Id);
        if (storagePlace is null)
        {
            return Errors.StoragePlaceForOrderNotFound();
        }

        storagePlace.ClearOrder();
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Рассчитать время, которое курьер потратит на путь до локации
    /// </summary>
    /// <param name="location">Локация</param>
    /// <returns>Время, которое курьер потратит на путь до локации</returns>
    public Result<double, Error> CalculateTimeToLocation(Location location)
    {
        var distanceResult = Location.DistanceBetween(location);
        if (distanceResult.IsFailure)
        {
            return distanceResult.Error;
        }

        return distanceResult.Value / Speed;
    }

    /// <summary>
    /// Изменить местоположение
    /// </summary>
    /// <param name="target">Целевое местоположение</param>
    public UnitResult<Error> Move(Location target)
    {
        if (target == null)
        {
            return GeneralErrors.ValueIsRequired(nameof(target));
        }

        var difX = target.X - Location.X;
        var difY = target.Y - Location.Y;
        var cruisingRange = Speed;

        var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
        cruisingRange -= Math.Abs(moveX);

        var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

        var locationCreateResult = Location.Create(Location.X + moveX, Location.Y + moveY);
        if (locationCreateResult.IsFailure)
        {
            return locationCreateResult.Error;
        }

        Location = locationCreateResult.Value;

        return UnitResult.Success<Error>();
    }

    private static UnitResult<Error> Validate(string name, int value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GeneralErrors.ValueIsInvalid(nameof(name));
        }

        if (value <= 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(value));
        }

        return UnitResult.Success<Error>();
    }

    private static class Errors
    {
        public static Error CourierOccupied()
        {
            return new Error(
                "courier.occupied",
                "Курьер не может взять заказ, так как все места его хранения заняты"
            );
        }

        public static Error StoragePlaceForOrderNotFound()
        {
            return new Error(
                "storage.place.for.order.not.found",
                "Место хранения для заказа не найдено"
            );
        }
    }
}