using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using DeliveryApp.Core.Ports.Repositories;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;

/// <summary>
/// Обработчик для <see cref="CreateOrderCommand"/>
/// </summary>
public class CreateOrderHandler(
    IUnitOfWork unitOfWork, 
    IOrderRepository orderRepository,
    IGeoClient geoClient) : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    /// <inheritdoc />
    public async Task<UnitResult<Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var getOrderResult = await orderRepository.GetAsync(message.OrderId);
        if (getOrderResult.HasValue)
        {
            return UnitResult.Success<Error>();
        }

        var getLocationResult = await geoClient.GetLocationAsync(message.Street, cancellationToken);
        if (getLocationResult.IsFailure)
        {
            return getLocationResult.Error;
        }

        var location = getLocationResult.Value;
        var orderCreateResult = Order.Create(message.OrderId, location, message.Volume);
        if (orderCreateResult.IsFailure)
        {
            return orderCreateResult;
        }

        var order = orderCreateResult.Value;
        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}