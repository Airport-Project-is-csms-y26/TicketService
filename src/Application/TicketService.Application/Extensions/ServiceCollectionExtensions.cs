using Microsoft.Extensions.DependencyInjection;
using TicketService.Application.Contracts.Tickets;
using TicketService.Application.Tickets;

namespace TicketService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<ITicketsService, TicketsService>();
        return collection;
    }
}