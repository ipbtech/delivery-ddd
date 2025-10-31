namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

/// <summary>
/// Модель ответа <see cref="GetAllCouriersQuery"/>
/// </summary>
public class GetAllCouriersResponse
{
    /// <summary>
    /// Модель ответа <see cref="GetAllCouriersQuery"/>
    /// </summary>
    /// <param name="couriers">Список курьеров</param>
    public GetAllCouriersResponse(List<CourierDto> couriers)
    {
        Couriers = couriers;
    }

    /// <summary>
    /// Курьеры
    /// </summary>
    public List<CourierDto> Couriers { get; set; }
}