namespace TicketService.Application.Contracts.Tickets.Operations;

public record GetTickets(int PageSize, int Cursor, long[] Ids, long[] PassengerIds, long[] FlightIds);