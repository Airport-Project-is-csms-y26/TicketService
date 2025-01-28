using FluentMigrator;

namespace TicketService.Infrastructure.Persistence.Migrations;

[Migration(1, "Initial")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        string sql = """
         CREATE TABLE Tickets (
             ticket_id bigint primary key generated always as identity,
             ticket_flight_id BIGINT NOT NULL,
             ticket_passenger_id BIGINT NOT NULL,
             ticket_place BIGINT NOT NULL,
             ticket_registered boolean not null
         );
        """;
        Execute.Sql(sql);
    }

    public override void Down()
    {
        Execute.Sql("""
        DROP TABLE Tickets;
        """);
    }
}