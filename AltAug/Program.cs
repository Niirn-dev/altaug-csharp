// See https://aka.ms/new-console-template for more information
using AltAug.Domain.Extensions;
using AltAug.Domain.Interfaces;
using AltAug.UI;
using AltAug.UI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.RegisterDomainServices();
        services.RegisterUIServices();
    })
    .Build();

var stateManager = host.Services.GetRequiredService<IStateManager>();

Console.WriteLine("Starting GUI...");

UIServices.Build(args, host.Services);

Console.WriteLine("GUI shut down. Saving application state...");
stateManager.Save();
