using Google.Protobuf.WellKnownTypes;
using Passengers.PassengerService.Contracts;
using PassengerService.GrpcClient.Clients.Interfaces;
using System.Runtime.CompilerServices;

namespace TicketService.Presentation.PassengerGrpcClient.Clients;

public class PassengerClient : IPassengerClient
{
    private readonly Passengers.PassengerService.Contracts.PassengerService.PassengerServiceClient _client;

    public PassengerClient(Passengers.PassengerService.Contracts.PassengerService.PassengerServiceClient client)
    {
        _client = client;
    }

    public async Task<CreatePassengerResponse> CreatePassenger(
        string name,
        long passport,
        string email,
        DateTimeOffset birthday,
        CancellationToken cancellationToken)
    {
        var request = new CreatePassengerRequest
        {
            Name = name,
            Passport = passport,
            Email = email,
            Birthday = Timestamp.FromDateTimeOffset(birthday),
        };

        return await _client.CreateAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<BanPassengerResponse> BanPassenger(long id, CancellationToken cancellationToken)
    {
        var request = new BanPassengerRequest
        {
            Id = id,
        };

        return await _client.BanAsync(request, cancellationToken: cancellationToken);
    }

    public async IAsyncEnumerable<Passenger> GetPassengers(
        int cursor,
        int pageSize,
        long[] passengerIds,
        long[] passportIds,
        string[] emails,
        string? name,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = new GetPassengersRequest
        {
            Cursor = cursor,
            PageSize = pageSize,
            PassengerIds = { passengerIds },
            PassportIds = { passportIds },
            Emails = { emails },
            Name = name,
        };

        GetPassengersResponse response =
            await _client.GetPassengersAsync(request, cancellationToken: cancellationToken);

        foreach (Passenger passenger in response.Passengers)
        {
            yield return passenger;
        }
    }
}