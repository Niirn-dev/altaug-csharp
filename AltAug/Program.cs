// See https://aka.ms/new-console-template for more information
using AltAug.Application.Extensions;
using AltAug.Domain.Extensions;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI;
using AltAug.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((ctx, builder) =>
    {
        builder.ClearProviders()
            .AddConsole();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.RegisterDomainServices()
            .RegisterApplicationServices()
            .RegisterUIServices();
    })
    .Build();

var stateManager = host.Services.GetRequiredService<IStateManager<AppConfig>>();

Console.WriteLine("Starting GUI...");

try
{
    UIServices.Build(args, host.Services);

    Console.WriteLine("GUI shut down. Saving application state...");
}
catch
{
    Console.WriteLine("Application ran into an unhandled issue, shutting down...");
}
finally
{
    stateManager.Save();
}
