namespace PassengerService.Application.Models.Tickets;

public record Ticket(long Id, long FlightId, long PassengerId, long Place, bool IsRegistred);