using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

/// <summary>
/// Заказ
/// </summary>
public sealed class Order : Aggregate<Guid>
{
    // ctr for EF Core
    private Order()
    { }

    private Order(Guid orderId, Location location, int volume) : this()
    {
        Id = orderId;
        Location = location;
        Volume = volume;
        Status = OrderStatus.Created;
    }

    /// <summary>
    /// Местоположение, куда нужно доставить заказ
    /// </summary>
    public Location Location { get; private set; }

    /// <summary>
    /// Объем
    /// </summary>
    public int Volume { get; private set; }

    /// <summary>
    /// Статус
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Идентификатор исполнителя (курьера)
    /// </summary>
    public Guid? CourierId { get; private set; }

    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="location">Геопозиция</param>
    /// <param name="volume">Объем</param>
    /// <returns>Новый заказ</returns>
    public static Result<Order, Error> Create(Guid orderId, Location location, int volume)
    {
        if (orderId == Guid.Empty)
        {
            return GeneralErrors.ValueIsRequired(nameof(orderId));
        }

        if (location == null)
        {
            return GeneralErrors.ValueIsRequired(nameof(location));
        }

        if (volume <= 0)
        {
            return GeneralErrors.ValueIsRequired(nameof(volume));
        }

        return new Order(orderId, location, volume);
    }

    /// <summary>
    /// Назначить заказ на курьера
    /// </summary>
    /// <param name="courier">Курьер</param>
    public UnitResult<Error> Assign(Courier courier)
    {
        if (courier is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(courier));
        }

        if (Status != OrderStatus.Created)
        {
            return Errors.ErrCantAssignAlreadyAssignedOrder(courier.Id);
        }

        CourierId = courier.Id;
        Status = OrderStatus.Assigned;

        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Завершить выполнение заказа
    /// </summary>
    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned)
        {
            return Errors.ErrCantCompleteNotAssignedOrder();
        }

        if (CourierId == null)
        {
            return Errors.ErrCantCompleteNotAssignedOrder();
        }

        Status = OrderStatus.Completed;
        return UnitResult.Success<Error>();
    }

    private static class Errors
    {
        public static Error ErrCantCompleteNotAssignedOrder()
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.complete.not.assigned.order",
                "Нельзя завершить заказ, который не был назначен");
        }

        public static Error ErrCantAssignAlreadyAssignedOrder(Guid courierId)
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.assign.already.assigned.order",
                $"Нельзя назначить уже назначенный заказ {courierId} ");
        }
    }
}