using AltAug.Application.Services;
using AltAug.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AltAug.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection that) => that
        .AddTransient<IAutomationService, AutomationService>();
}
