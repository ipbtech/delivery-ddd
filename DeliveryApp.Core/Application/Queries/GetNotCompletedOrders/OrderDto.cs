using DeliveryApp.Core.Application.Queries.CommonDto;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

/// <summary>
/// Dto заказа
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Геопозиция
    /// </summary>
    public LocationDto Location { get; set; }
}