using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports.Repositories;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCouriers;

/// <summary>
/// Обработчик для <see cref="MoveCouriersCommand"/>
/// </summary>
public class MoveCouriersHandler(
    IUnitOfWork unitOfWork,
    IOrderRepository orderRepository,
    ICourierRepository courierRepository) : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
{
    /// <inheritdoc />
    public async Task<UnitResult<Error>> Handle(MoveCouriersCommand message, CancellationToken cancellationToken)
    {
        var assignedOrders = await orderRepository.GetAllInAssignedStatusAsync();
        if (!assignedOrders.Any())
        {
            return UnitResult.Success<Error>();
        }

        foreach (var order in assignedOrders)
        {
            if (!order.CourierId.HasValue)
            {
                return GeneralErrors.ValueIsInvalid(nameof(order.CourierId));
            }

            var getCourierResult = await courierRepository.GetAsync(order.CourierId.Value);
            if (getCourierResult.HasNoValue)
            {
                return GeneralErrors.ValueIsInvalid(nameof(order.CourierId));
            }
            var courier = getCourierResult.Value;

            // Перемещаем курьера
            var courierMoveResult = courier.Move(order.Location);
            if (courierMoveResult.IsFailure)
            {
                return courierMoveResult;
            }

            // Если курьер дошел до точки заказа - завершаем заказ, освобождаем курьера
            if (order.Location == courier.Location)
            {
                order.Complete();
                courier.CompleteOrder(order);
            }

            courierRepository.Update(courier);
            orderRepository.Update(order);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }
}