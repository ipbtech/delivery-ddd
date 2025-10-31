using DeliveryApp.Core.Application.Queries.CommonDto;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

/// <summary>
/// Dto курьера
/// </summary>
public class CourierDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Геопозиция
    /// </summary>
    public LocationDto Location { get; set; }
}