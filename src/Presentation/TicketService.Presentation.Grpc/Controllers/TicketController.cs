using Grpc.Core;
using System.Diagnostics;
using Tickets.TicketsService.Contracts;
using TicketService.Application.Contracts.Tickets;
using TicketService.Application.Contracts.Tickets.Operations;
using Ticket = PassengerService.Application.Models.Tickets.Ticket;

namespace TicketService.Presentation.Grpc.Controllers;

public class TicketController : TicketsService.TicketsServiceBase
{
    private readonly ITicketsService _ticketsService;

    public TicketController(ITicketsService ticketsService)
    {
        _ticketsService = ticketsService;
    }

    public override async Task<CreateTicketResponse> Create(CreateTicketRequest request, ServerCallContext context)
    {
        var createTicketRequest = new CreateTicket.Request(
            request.PassengerId,
            request.FlightId,
            request.Place);

        CreateTicket.Result response = await _ticketsService.CreateAsync(
            createTicketRequest,
            context.CancellationToken);
        return response switch
        {
            CreateTicket.Result.Success => new CreateTicketResponse(),

            CreateTicket.Result.PassengerNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Passenger was not found!")),

            CreateTicket.Result.FlightNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Flight was not found!")),

            CreateTicket.Result.InvalidPlaceNumber => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "Negative place number!")),

            CreateTicket.Result.TakenPlace => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "Place is taken!")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<RegisterPassengerOnFlightResponse> RegisterPassengerOnFlight(
        RegisterPassengerOnFlightRequest request,
        ServerCallContext context)
    {
        var registerPassengerOnFlightRequest = new RegisterPassengerOnFlight.Request(request.TicketId);

        RegisterPassengerOnFlight.Result response = await _ticketsService.RegisterPassengerOnFlightAsync(
            registerPassengerOnFlightRequest,
            context.CancellationToken);
        return response switch
        {
            TicketService.Application.Contracts.Tickets.Operations.RegisterPassengerOnFlight.Result.Success =>
                new RegisterPassengerOnFlightResponse(),

            TicketService.Application.Contracts.Tickets.Operations.RegisterPassengerOnFlight.Result
                .TicketNotFound => throw new
                RpcException(new Status(
                    StatusCode.NotFound,
                    "Ticket was not found!")),

            TicketService.Application.Contracts.Tickets.Operations.RegisterPassengerOnFlight.Result
                .InvalidData => throw new
                RpcException(new Status(
                    StatusCode.NotFound,
                    "Invalid data! Passenger was not found!")),

            TicketService.Application.Contracts.Tickets.Operations.RegisterPassengerOnFlight.Result
                .PassengerBanned => throw new
                RpcException(new Status(
                    StatusCode.PermissionDenied,
                    "Passenger was banned!")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<GetTicketsResponse> GetTickets(GetTicketsRequest request, ServerCallContext context)
    {
        var getTicketsRequest = new GetTickets(
            request.PageSize,
            request.Cursor,
            request.TicketIds.ToArray(),
            request.PassengerIds.ToArray(),
            request.FlightIds.ToArray());

        IAsyncEnumerable<Ticket> tickets = _ticketsService.GetTickets(getTicketsRequest, context.CancellationToken);

        var response = new GetTicketsResponse();
        await foreach (Ticket ticket in tickets)
        {
            response.Tickets.Add(new Tickets.TicketsService.Contracts.Ticket()
            {
                TicketId = ticket.Id,
                TicketFlightId = ticket.FlightId,
                TicketPassengerId = ticket.PassengerId,
                TicketPlace = ticket.Place,
                TicketRegistered = ticket.IsRegistred,
            });
        }

        return response;
    }
}