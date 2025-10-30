using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

/// <summary>
/// Получить все незавершенные заказы (со статусом Created и Assigned)
/// </summary>
public class GetNotCompletedOrdersQuery : IRequest<GetNotCompletedOrdersResponse>;