namespace TicketService.Application.Contracts.Tickets.Operations;

public static class RegisterPassengerOnFlight
{
    public readonly record struct Request(long TicketId);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record TicketNotFound : Result;

        public sealed record InvalidData : Result;

        public sealed record PassengerBanned : Result;
    }
}