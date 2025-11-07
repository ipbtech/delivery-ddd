using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Ports;

/// <summary>
/// Клиент для работы с сервисом Geo
/// </summary>
public interface IGeoClient
{
    /// <summary>
    /// Получить информацию о геолокации
    /// </summary>
    /// <param name="street">Название улицы</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<Result<Location, Error>> GetLocationAsync(string street, CancellationToken cancellationToken = default);
}