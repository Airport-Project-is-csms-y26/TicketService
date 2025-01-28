using Passengers.PassengerService.Contracts;

namespace PassengerService.GrpcClient.Clients.Interfaces;

public interface IPassengerClient
{
    Task<CreatePassengerResponse> CreatePassenger(
        string name,
        long passport,
        string email,
        DateTimeOffset birthday,
        CancellationToken cancellationToken);

    Task<BanPassengerResponse> BanPassenger(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<Passenger> GetPassengers(
        int cursor,
        int pageSize,
        long[] passengerIds,
        long[] passportIds,
        string[] emails,
        string? name,
        CancellationToken cancellationToken);
}