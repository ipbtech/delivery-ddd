using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers;

/// <summary>
/// Получить всех курьеров
/// </summary>
public class GetAllCouriersQuery : IRequest<GetAllCouriersResponse>;