using Npgsql;
using PassengerService.Application.Models.Tickets;
using System.Runtime.CompilerServices;
using TicketService.Application.Abstractions.Persistence.Queries;
using TicketService.Application.Abstractions.Persistence.Repositories;

namespace TicketService.Infrastructure.Persistence.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public TicketsRepository(NpgsqlDataSource dataSource)
    {
        _npgsqlDataSource = dataSource;
    }

    public async Task CreateTicket(long passengerId, long flightId, long place, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO tickets (ticket_passenger_id, ticket_flight_id, ticket_place, ticket_registered)
                           VALUES (@PassengerId, @FlightId, @Place, false)
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("PassengerId", passengerId),
                new NpgsqlParameter("FlightId", flightId),
                new NpgsqlParameter("Place", place),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RegisterPassengerOnFlight(long ticketId, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE tickets
                           SET ticket_registered = true
                           WHERE ticket_id = @TicketId;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("TicketId", ticketId),
            },
        };
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Ticket?> GetTicketById(long id, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT ticket_id,
                                  ticket_flight_id,
                                  ticket_passenger_id,
                                  ticket_place,
                                  ticket_registered
                           FROM tickets
                           WHERE ticket_id = @Id;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("Id", id),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new Ticket(
            reader.GetInt64(reader.GetOrdinal("ticket_id")),
            reader.GetInt64(reader.GetOrdinal("ticket_flight_id")),
            reader.GetInt64(reader.GetOrdinal("ticket_passenger_id")),
            reader.GetInt64(reader.GetOrdinal("ticket_place")),
            reader.GetBoolean(reader.GetOrdinal("ticket_registered")));
    }

    public async IAsyncEnumerable<Ticket> GetTicketsAsync(
        GetTicketsQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT ticket_id,
                                  ticket_flight_id,
                                  ticket_passenger_id,
                                  ticket_place,
                                  ticket_registered
                           FROM tickets
                           WHERE (ticket_id > @Cursor)
                           AND (cardinality(@Ids) = 0 OR ticket_id = any(@Ids))
                           AND (cardinality(@FlightIds) = 0 OR ticket_flight_id = any(@FlightIds))
                           AND (cardinality(@PassengerIds) = 0 OR ticket_passenger_id = any(@PassengerIds))
                           LIMIT @Limit;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("Cursor", query.Cursor),
                new NpgsqlParameter("Limit", query.PageSize),
                new NpgsqlParameter("Ids", query.Ids),
                new NpgsqlParameter("FlightIds", query.FlightIds),
                new NpgsqlParameter("PassengerIds", query.PassengerIds),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Ticket(
                reader.GetInt64(reader.GetOrdinal("ticket_id")),
                reader.GetInt64(reader.GetOrdinal("ticket_flight_id")),
                reader.GetInt64(reader.GetOrdinal("ticket_passenger_id")),
                reader.GetInt64(reader.GetOrdinal("ticket_place")),
                reader.GetBoolean(reader.GetOrdinal("ticket_registered")));
        }
    }

    public async IAsyncEnumerable<long> GetTakenPlaces(long flightId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT ticket_place
                           FROM tickets
                           WHERE ticket_flight_id = @FlightId;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("FlightId", flightId),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return reader.GetInt64(reader.GetOrdinal("ticket_place"));
        }
    }
}