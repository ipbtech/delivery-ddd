using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

/// <inheritdoc />
public class DispatchService : IDispatchService
{
    /// <inheritdoc />
    public Result<Courier, Error> Dispatch(List<Courier> couriers, Order order)
    {
        if (order.Status != OrderStatus.Created)
        {
            return Errors.OrderStatusIsNotCreated(order.Id);
        }

        var selectedCourier = couriers
            .Select(x => new
            {
                Courier = x,
                DeliveryTimeResult = x.CalculateTimeToLocation(order.Location),
                CanTakeOrderResult = x.CanTakeOrder(order)
            })
            .Where(x => x.DeliveryTimeResult.IsSuccess && x.CanTakeOrderResult is { IsSuccess: true, Value: true })
            .OrderBy(x => x.DeliveryTimeResult.Value)
            .Select(x => x.Courier)
            .FirstOrDefault();

        if (selectedCourier is null)
        {
            return GeneralErrors.NotFound();
        }

        var orderAssignResult = order.Assign(selectedCourier);
        if (orderAssignResult.IsFailure)
        {
            return Result.Failure<Courier, Error>(orderAssignResult.Error);
        }

        var takeOrderResult = selectedCourier.TakeOrder(order);
        if (takeOrderResult.IsFailure)
        {
            return Result.Failure<Courier, Error>(takeOrderResult.Error);
        }

        return selectedCourier;
    }

    private static class Errors
    {
        public static Error OrderStatusIsNotCreated(Guid orderId)
        {
            return new Error(
                "order.status.is.not.created",
                $"Статус заказа {orderId} не соответствует статусу {nameof(OrderStatus.Created)}"
                );
        }
    }
}