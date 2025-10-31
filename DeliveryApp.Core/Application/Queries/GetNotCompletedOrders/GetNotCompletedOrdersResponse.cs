namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

/// <summary>
/// Модель ответа для <see cref="GetNotCompletedOrdersQuery"/>
/// </summary>
public class GetNotCompletedOrdersResponse
{
    /// <summary>
    /// Модель ответа для <see cref="GetNotCompletedOrdersQuery"/>
    /// </summary>
    /// <param name="orders">Заказы</param>
    public GetNotCompletedOrdersResponse(List<OrderDto> orders)
    {
        Orders = orders;
    }

    /// <summary>
    /// Заказы
    /// </summary>
    public List<OrderDto> Orders { get; set; }
}