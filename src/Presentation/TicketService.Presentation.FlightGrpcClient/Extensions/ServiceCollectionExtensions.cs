using Flights.FlightsService.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TicketService.Presentation.FlightGrpcClient.Clients;
using TicketService.Presentation.FlightGrpcClient.Clients.Interfaces;
using TicketService.Presentation.FlightGrpcClient.Options;

namespace TicketService.Presentation.FlightGrpcClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlightServiceGrpcClient(
        this IServiceCollection collection)
    {
        collection.AddGrpcClient<FlightsService.FlightsServiceClient>((sp, o) =>
        {
            IOptions<FlightServiceClientOptions> options = sp.GetRequiredService<IOptions<FlightServiceClientOptions>>();
            o.Address = new Uri(options.Value.GrpcServerUrl);
        });

        collection.AddScoped<IFlightClient, FlightClient>();
        return collection;
    }
}