using PassengerService.Application.Models.Tickets;
using TicketService.Application.Contracts.Tickets.Operations;

namespace TicketService.Application.Contracts.Tickets;

public interface ITicketsService
{
    Task<CreateTicket.Result> CreateAsync(CreateTicket.Request request, CancellationToken cancellationToken);

    Task<RegisterPassengerOnFlight.Result> RegisterPassengerOnFlightAsync(
        RegisterPassengerOnFlight.Request request,
        CancellationToken cancellationToken);

    IAsyncEnumerable<Ticket> GetTickets(GetTickets request, CancellationToken cancellationToken);
}