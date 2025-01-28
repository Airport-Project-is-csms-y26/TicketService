using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using TicketService.Application.Abstractions.Persistence.Repositories;
using TicketService.Infrastructure.Persistence.Migrations;
using TicketService.Infrastructure.Persistence.Options;
using TicketService.Infrastructure.Persistence.Repositories;

namespace TicketService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigration(
        this IServiceCollection collection)
    {
        collection
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(serviceProvider =>
                    serviceProvider
                        .GetRequiredService<IOptions<PostgresOptions>>()
                        .Value
                        .GetConnectionString())
                .ScanIn(typeof(InitialMigration).Assembly));

        collection.AddSingleton<NpgsqlDataSource>(provider =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(provider
                .GetRequiredService<IOptions<PostgresOptions>>()
                .Value
                .GetConnectionString());

            return dataSourceBuilder.Build();
        });

        return collection;
    }

    public static IServiceCollection AddInfrastructureDataAccess(this IServiceCollection collection)
    {
        collection.AddScoped<ITicketsRepository, TicketsRepository>();

        return collection;
    }
}