#pragma warning disable CA1506

using TicketService.Application.BackgroundServices;
using TicketService.Application.Extensions;
using TicketService.Infrastructure.Persistence.Extensions;
using TicketService.Infrastructure.Persistence.Options;
using TicketService.Presentation.FlightGrpcClient.Extensions;
using TicketService.Presentation.FlightGrpcClient.Options;
using TicketService.Presentation.Grpc.Controllers;
using TicketService.Presentation.Grpc.Interceptors;
using TicketService.Presentation.PassengerGrpcClient.Extensions;
using TicketService.Presentation.PassengerGrpcClient.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

builder.Services.Configure<PostgresOptions>(builder.Configuration.GetSection("Persistence:Postgres"));
builder.Services.Configure<PassengerServiceClientOptions>(builder.Configuration.GetSection("PassengerClient"));
builder.Services.Configure<FlightServiceClientOptions>(builder.Configuration.GetSection("FlightClient"));

builder.Services
    .AddMigration()
    .AddInfrastructureDataAccess()
    .AddApplication();

builder.Services.AddHostedService<MigrationService>();

builder.Services.AddGrpc(o => o.Interceptors.Add<ExceptionInterceptor>());

builder.Services
    .AddPassengerGrpcClient()
    .AddFlightServiceGrpcClient();
WebApplication app = builder.Build();

app.MapGrpcService<TicketController>();
app.Run();