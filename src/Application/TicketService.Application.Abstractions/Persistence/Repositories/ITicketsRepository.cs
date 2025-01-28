using PassengerService.Application.Models.Tickets;
using TicketService.Application.Abstractions.Persistence.Queries;

namespace TicketService.Application.Abstractions.Persistence.Repositories;

public interface ITicketsRepository
{
    Task CreateTicket(long passengerId, long flightId, long place, CancellationToken cancellationToken);

    Task RegisterPassengerOnFlight(long ticketId, CancellationToken cancellationToken);

    Task<Ticket?> GetTicketById(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<Ticket> GetTicketsAsync(GetTicketsQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<long> GetTakenPlaces(long flightId, CancellationToken cancellationToken);
}