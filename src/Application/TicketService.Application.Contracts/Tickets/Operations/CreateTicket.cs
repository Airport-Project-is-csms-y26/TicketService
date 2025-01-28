namespace TicketService.Application.Contracts.Tickets.Operations;

public static class CreateTicket
{
    public readonly record struct Request(long PassengerId, long FlightId, long Place);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record InvalidPlaceNumber : Result;

        public sealed record PassengerNotFound : Result;

        public sealed record FlightNotFound : Result;

        public sealed record TakenPlace : Result;
    }
}