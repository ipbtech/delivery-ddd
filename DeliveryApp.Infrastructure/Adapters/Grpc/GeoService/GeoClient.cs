using Clients.Geo;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using Primitives;
using Location = DeliveryApp.Core.Domain.Models.SharedKernel.Location;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

/// <inheritdoc />
public class GeoClient(Geo.GeoClient geoClientService) : IGeoClient
{
    /// <inheritdoc />
    public async Task<Result<Location, Error>> GetLocationAsync(string street, CancellationToken cancellationToken = default)
    {
        var reply = await geoClientService.GetGeolocationAsync(
            new GetGeolocationRequest { Street = street }, 
            null, 
            DateTime.UtcNow.AddSeconds(2), 
            cancellationToken);

        var locationCreateResult = Location.Create(reply.Location.X, reply.Location.Y);
        if (locationCreateResult.IsFailure)
        {
            return locationCreateResult;
        }

        return locationCreateResult.Value;
    }
}