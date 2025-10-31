﻿using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;

/// <summary>
/// Создать заказ
/// </summary>
public class CreateOrderCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    /// Идентификатор заказа
    /// </summary>
    public Guid OrderId { get; }

    /// <summary>
    /// Улица
    /// </summary>
    /// <remarks>Корзина содержала полный Address, но для упрощения мы будем использовать только Street из Address</remarks>
    public string Street { get; }

    /// <summary>
    /// Объем
    /// </summary>
    public int Volume { get; }

    private CreateOrderCommand(Guid orderId, string street, int volume)
    {
        OrderId = orderId;
        Street = street;
        Volume = volume;
    }
    
    /// <summary>
    /// Создать заказ
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="street">Улица</param>
    /// <param name="volume">Объем</param>
    public static Result<CreateOrderCommand, Error> Create(Guid orderId, string street, int volume)
    {
        if (orderId == Guid.Empty)
        {
            return GeneralErrors.ValueIsRequired(nameof(orderId));
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            return GeneralErrors.ValueIsRequired(nameof(street));
        }

        if (volume <= 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(volume));
        }

        return new CreateOrderCommand(orderId, street, volume);
    }
}