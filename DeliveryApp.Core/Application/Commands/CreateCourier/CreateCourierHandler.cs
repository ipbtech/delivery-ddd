using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Npgsql;
using Primitives;
using System.Data;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports.Repositories;

namespace DeliveryApp.Core.Application.Commands.CreateCourier;

/// <summary>
/// Обработчик для <see cref="CreateCourierCommand"/>
/// </summary>
public class CreateCourierHandler(
    IUnitOfWork unitOfWork,
    ICourierRepository courierRepository) : IRequestHandler<CreateCourierCommand, UnitResult<Error>>
{
    /// <inheritdoc />
    public async Task<UnitResult<Error>> Handle(CreateCourierCommand message, CancellationToken cancellationToken)
    {
        var location = Location.CreateRandom().Value;
        var courier = Courier.Create(message.Name, message.Speed, location).Value;

        await courierRepository.AddAsync(courier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}