using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCouriers;

/// <summary>
/// Переместить курьеров
/// </summary>
public class MoveCouriersCommand : IRequest<UnitResult<Error>>;