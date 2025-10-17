using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

/// <summary>
/// Место хранения заказа курьером
/// </summary>
public sealed class StoragePlace : Entity<Guid>
{
    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Полный объем
    /// </summary>
    public int TotalVolume { get; }

    /// <summary>
    /// Идентификатор заказа
    /// </summary>
    public Guid? OrderId { get; private set; }

    /// <summary>
    /// Занято ли место хранения другим заказом
    /// </summary>
    public bool IsOccupied => OrderId.HasValue;

    // конструктор для EF Core
    private StoragePlace()
    { }

    private StoragePlace(string name, int totalVolume) : this()
    {
        Id = Guid.NewGuid();
        Name = name;
        TotalVolume = totalVolume;
    }

    /// <summary>
    /// Создать место хранения заказа
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="totalVolume">Полный объем</param>
    /// <returns>Новое место хранения</returns>
    public static Result<StoragePlace, Error> Create(string name, int totalVolume)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GeneralErrors.ValueIsInvalid(nameof(name));
        }

        if (totalVolume <= 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(totalVolume));
        }
        
        return new StoragePlace(name, totalVolume);
    }

    /// <summary>
    /// Убедиться, можно ли поместить заказ в место хранения
    /// </summary>
    /// <param name="orderVolume"></param>
    /// <returns>True - если да, False - если нет</returns>
    public Result<bool, Error> CanStore(int orderVolume)
    {
        if (IsOccupied)
        {
            return false;
        }
        
        if (orderVolume <= 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(orderVolume));
        }

        if (orderVolume > TotalVolume)
        {
            return Errors.OrderVolumeGreaterThanVolumeOfStorage();
        }

        return true;
    }

    /// <summary>
    /// Расположить заказа в место хранения
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="orderVolume">Объем заказа</param>
    public UnitResult<Error> StoreOrder(Guid orderId, int orderVolume)
    {
        var checkStoreResult = CanStore(orderVolume);
        if (checkStoreResult.IsFailure || !checkStoreResult.Value)
        {
            return Errors.StorageIsOccupied();
        }

        OrderId = orderId;
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Очистить место хранения от заказов
    /// </summary>
    public void ClearOrder()
    {
        if (IsOccupied)
        {
            OrderId = null;
        }
    }

    private static class Errors
    {
        public static Error OrderVolumeGreaterThanVolumeOfStorage()
        {
            return new Error("order.volume.greater.then.storage.volume",
                "Объем заказа не должен превышать объем места хранения");
        }

        public static Error StorageIsOccupied()
        {
            return new Error("storage.is.occupied",
                "Место хранения заказов занято");
        }
    }
}