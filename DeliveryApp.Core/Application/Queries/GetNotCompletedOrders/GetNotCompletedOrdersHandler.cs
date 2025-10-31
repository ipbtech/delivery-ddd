using Dapper;
using DeliveryApp.Core.Application.Queries.CommonDto;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

/// <summary>
/// Обработчик для <see cref="GetNotCompletedOrdersQuery"/>
/// </summary>
public class GetNotCompletedOrdersHandler(string connectionString) : IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>
{
    private readonly string _connectionString = 
        !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));

    /// <inheritdoc />
    public async Task<GetNotCompletedOrdersResponse> Handle(GetNotCompletedOrdersQuery message, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var result = await connection.QueryAsync<dynamic>(
            @"SELECT id, location_x, location_y FROM public.orders where status!=@status;"
            , new { status = OrderStatus.Completed.Name });

        var resultList = result.ToList();
        if (!resultList.Any())
        {
            return null;
        }

        var orders = resultList.Select(x => new OrderDto()
        {
            Id = x.id,
            Location = new LocationDto()
            {
                X = x.location_x,
                Y = x.location_y
            }
        }).ToList();

        return new GetNotCompletedOrdersResponse(orders);
    }
}