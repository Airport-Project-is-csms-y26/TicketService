using Flights.FlightsService.Contracts;
using Passengers.PassengerService.Contracts;
using PassengerService.Application.Models.Tickets;
using PassengerService.GrpcClient.Clients.Interfaces;
using TicketService.Application.Abstractions.Persistence.Queries;
using TicketService.Application.Abstractions.Persistence.Repositories;
using TicketService.Application.Contracts.Tickets;
using TicketService.Application.Contracts.Tickets.Operations;
using TicketService.Presentation.FlightGrpcClient.Clients.Interfaces;

namespace TicketService.Application.Tickets;

public class TicketsService : ITicketsService
{
    private readonly ITicketsRepository _ticketsRepository;
    private readonly IPassengerClient _passengerClient;
    private readonly IFlightClient _flightClient;

    public TicketsService(
        ITicketsRepository ticketsRepository,
        IPassengerClient passengerClient,
        IFlightClient flightClient)
    {
        _ticketsRepository = ticketsRepository;
        _passengerClient = passengerClient;
        _flightClient = flightClient;
    }

    public async Task<CreateTicket.Result> CreateAsync(
        CreateTicket.Request request,
        CancellationToken cancellationToken)
    {
        Passenger[] passengers = await _passengerClient
            .GetPassengers(
                0,
                int.MaxValue,
                new[] { request.PassengerId },
                Array.Empty<long>(),
                Array.Empty<string>(),
                null,
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (passengers.Length == 0)
        {
            return new CreateTicket.Result.PassengerNotFound();
        }

        Flight[] flights = await _flightClient
            .GetFlights(int.MaxValue, 0, new[] { request.FlightId }, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (flights.Length == 0)
        {
            return new CreateTicket.Result.FlightNotFound();
        }

        if (request.Place < 0)
        {
            return new CreateTicket.Result.InvalidPlaceNumber();
        }

        long[] takenPlaces = await _ticketsRepository
            .GetTakenPlaces(request.FlightId, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (takenPlaces.Contains(request.Place))
        {
            return new CreateTicket.Result.TakenPlace();
        }

        await _ticketsRepository.CreateTicket(
            request.PassengerId,
            request.FlightId,
            request.Place,
            cancellationToken);

        return new CreateTicket.Result.Success();
    }

    public async Task<RegisterPassengerOnFlight.Result> RegisterPassengerOnFlightAsync(
        RegisterPassengerOnFlight.Request request,
        CancellationToken cancellationToken)
    {
        Ticket? ticket = await _ticketsRepository.GetTicketById(request.TicketId, cancellationToken);

        if (ticket == null)
        {
            return new RegisterPassengerOnFlight.Result.TicketNotFound();
        }

        Passenger[] passengers = await _passengerClient
            .GetPassengers(
                0,
                int.MaxValue,
                new[] { ticket.PassengerId },
                Array.Empty<long>(),
                Array.Empty<string>(),
                null,
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (passengers.Length == 0)
        {
            return new RegisterPassengerOnFlight.Result.InvalidData();
        }

        if (passengers[0].IsBanned)
        {
            return new RegisterPassengerOnFlight.Result.PassengerBanned();
        }

        await _ticketsRepository.RegisterPassengerOnFlight(request.TicketId, cancellationToken);

        return new RegisterPassengerOnFlight.Result.Success();
    }

    public IAsyncEnumerable<Ticket> GetTickets(GetTickets request, CancellationToken cancellationToken)
    {
        var query = new GetTicketsQuery(
            request.PageSize,
            request.Cursor,
            request.Ids,
            request.PassengerIds,
            request.FlightIds);

        return _ticketsRepository.GetTicketsAsync(query, cancellationToken);
    }
}