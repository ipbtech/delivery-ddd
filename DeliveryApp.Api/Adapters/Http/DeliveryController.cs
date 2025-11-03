using System.Net;
using DeliveryApp.Core.Application.Commands.CreateCourier;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;
using Order = OpenApi.Models.Order;
using Location = OpenApi.Models.Location;
using Courier = OpenApi.Models.Courier;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController(IMediator mediator) : DefaultApiController
{
    public override async Task<IActionResult> GetOrders()
    {
        var getActiveOrdersQuery = new GetNotCompletedOrdersQuery();
        var response = await mediator.Send(getActiveOrdersQuery);

        if (response is null)
        {
            return NotFound();
        }

        var model = response.Orders.Select(o => new Order
        {
            Id = o.Id,
            Location = new Location
            {
                X = o.Location.X, 
                Y = o.Location.Y
            }
        });

        return Ok(model);
    }

    public override async Task<IActionResult> CreateOrder()
    {
        var orderId = Guid.NewGuid();
        var street = "Несуществующая";

        var createOrderCommandResult = CreateOrderCommand.Create(orderId, street, 5);
        if (createOrderCommandResult.IsFailure)
        {
            return BadRequest(createOrderCommandResult.Error);
        }

        var response = await mediator.Send(createOrderCommandResult.Value);
        if (response.IsSuccess)
        {
            return Ok();
        }

        return Conflict(new ProblemDetails()
        {
            Status = (int)HttpStatusCode.Conflict,
            Detail = response.Error.Message
        });
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var getAllCouriersQuery = new GetAllCouriersQuery();
        var response = await mediator.Send(getAllCouriersQuery);

        if (response is null)
        {
            return NotFound();
        }

        var model = response.Couriers.Select(c => new Courier
        {
            Id = c.Id,
            Name = c.Name,
            Location = new Location
            {
                X = c.Location.X, 
                Y = c.Location.Y
            }
        });

        return Ok(model);
    }

    public override async Task<IActionResult> CreateCourier(NewCourier newCourier)
    {
        var createCourierCommand = CreateCourierCommand.Create(newCourier.Name, newCourier.Speed);
        var response = await mediator.Send(createCourierCommand.Value);
        if (response.IsSuccess)
        {
            return Ok();
        }

        return Conflict(new ProblemDetails()
        {
            Status = (int)HttpStatusCode.Conflict,
            Detail = response.Error.Message
        });
    }
}