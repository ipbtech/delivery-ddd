using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

/// <summary>
/// Сервис диспетчеризации заказов на курьеров
/// </summary>
public interface IDispatchService
{
    /// <summary>
    /// Найти подходящего курьера на заказ
    /// </summary>
    /// <param name="couriers">Список курьера</param>
    /// <param name="order">Заказ</param>
    /// <returns>Результат - Курьер, на которого назначается заказ</returns>
    Result<Courier, Error> Dispatch(List<Courier> couriers, Order order);
}