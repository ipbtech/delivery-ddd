using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports.Repositories;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.AssignOrder;

/// <summary>
/// Обработчик для <see cref="AssignOrderCommand"/>
/// </summary>
public class AssignOrderHandler(
    IUnitOfWork unitOfWork,
    IOrderRepository orderRepository,
    ICourierRepository courierRepository,
    IDispatchService dispatchService) : IRequestHandler<AssignOrderCommand, UnitResult<Error>>
{
    /// <inheritdoc />
    public async Task<UnitResult<Error>> Handle(AssignOrderCommand message, CancellationToken cancellationToken)
    {
        var getFirstInCreatedStatusResult = await orderRepository.GetFirstInCreatedStatusAsync();
        if (getFirstInCreatedStatusResult.HasNoValue)
        {
            return Errors.NotAvailableOrders();
        }
        var order = getFirstInCreatedStatusResult.Value;

        var availableCouriers = await courierRepository.GetAllFreeAsync();
        if (!availableCouriers.Any())
        {
            return Errors.NotAvailableCouriers();
        }

        var dispatchResult = dispatchService.Dispatch(availableCouriers, order);
        if (dispatchResult.IsFailure)
        {
            return dispatchResult;
        }
        var courier = dispatchResult.Value;

        courierRepository.Update(courier);
        orderRepository.Update(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    private static class Errors
    {
        public static Error NotAvailableOrders()
        {
            return new Error("Not Available Orders",
                "Нет заказов, для назначения");
        }

        public static Error NotAvailableCouriers()
        {
            return new Error("Not Available Couriers",
                "Нет доступных курьеров");
        }
    }
}