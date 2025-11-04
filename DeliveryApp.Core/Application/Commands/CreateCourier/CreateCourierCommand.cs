using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateCourier;

/// <summary>
/// Создать курьера
/// </summary>
public class CreateCourierCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Скорость
    /// </summary>
    public int Speed { get; }

    private CreateCourierCommand(string name, int speed)
    {
        Name = name;
        Speed = speed;
    }

    /// <summary>
    /// Создать курьера
    /// </summary>
    /// <param name="name">Идентификатор курьера</param>
    /// <param name="speed">Скорость</param>
    public static Result<CreateCourierCommand, Error> Create(string name, int speed)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GeneralErrors.ValueIsInvalid(nameof(name));
        }

        if (speed <= 0)
        {
            return GeneralErrors.ValueIsInvalid(nameof(speed));
        }

        return new CreateCourierCommand(name, speed);
    }
}