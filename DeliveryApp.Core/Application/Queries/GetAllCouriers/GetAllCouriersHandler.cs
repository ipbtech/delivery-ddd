using Dapper;
using DeliveryApp.Core.Application.Queries.CommonDto;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

/// <summary>
/// Обработчик для <see cref="GetAllCouriersQuery"/>
/// </summary>
/// <param name="connectionString">Строка подключения к БД</param>
public class GetAllCouriersHandler(string connectionString)
    : IRequestHandler<GetAllCouriersQuery, GetAllCouriersResponse>
{
    private readonly string _connectionString = 
        !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));

    /// <inheritdoc />
    public async Task<GetAllCouriersResponse> Handle(GetAllCouriersQuery message, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var result = await connection.QueryAsync<dynamic>(
            @"SELECT id, name, location_x, location_y FROM public.couriers"
            , new { });

        var resultList = result.ToList();

        if (!resultList.Any())
        {
            return null;
        }

        var couriers = resultList.Select(x => new CourierDto
        {
            Id = x.id, 
            Name = x.name, 
            Location = new LocationDto()
            {
                X = x.location_x,
                Y = x.location_y
            }
        }).ToList();

        return new GetAllCouriersResponse(couriers);
    }
}