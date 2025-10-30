using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports.Repositories;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;

/// <summary>
/// Обработчик для <see cref="CreateOrderCommand"/>
/// </summary>
public class CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    /// <inheritdoc />
    public async Task<UnitResult<Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var getOrderResult = await orderRepository.GetAsync(message.OrderId);
        if (getOrderResult.HasValue)
        {
            return UnitResult.Success<Error>();
        }

        // TODO: в будущем будем получать из другого сервиса Geo
        var location = Location.CreateRandom().Value;

        // Создаем заказ
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