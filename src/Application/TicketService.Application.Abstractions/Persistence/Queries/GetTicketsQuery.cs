namespace TicketService.Application.Abstractions.Persistence.Queries;

public record GetTicketsQuery(
    int PageSize,
    int Cursor,
    long[] Ids,
    long[] PassengerIds,
    long[] FlightIds);