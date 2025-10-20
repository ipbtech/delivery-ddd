using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate;

/// <summary>
/// Статус заказа
/// </summary>
public class OrderStatus : ValueObject
{
    /// <summary>
    /// Создан
    /// </summary>
    public static OrderStatus Created => new(nameof(Created).ToLowerInvariant());

    /// <summary>
    /// Назначен
    /// </summary>
    public static OrderStatus Assigned => new(nameof(Assigned).ToLowerInvariant());

    /// <summary>
    /// Выполнен
    /// </summary>
    public static OrderStatus Completed => new(nameof(Completed).ToLowerInvariant());

    // ctr for EF Core
    private OrderStatus()
    { }

    private OrderStatus(string name) : this()
    {
        Name = name;
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}