namespace DeliveryApp.Infrastructure;

/// <summary>
/// Опции приложения
/// </summary>
public class Settings
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Адрес подключения к сервису геолокаций по Grpc
    /// </summary>
    public string GeoServiceGrpcHost { get; set; }

    /// <summary>
    /// Адрес подключения к Кафке
    /// </summary>
    public string MessageBrokerHost { get; set; }

    /// <summary>
    /// Имя топика для изменения статуса заказов
    /// </summary>
    public string OrderStatusChangedTopic { get; set; }

    /// <summary>
    /// Имя топика для подтверждения корзины
    /// </summary>
    public string BasketConfirmedTopic { get; set; }
}