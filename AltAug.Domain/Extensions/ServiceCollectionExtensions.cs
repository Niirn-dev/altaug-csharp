using AltAug.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AltAug.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection that) => that
        .AddSingleton<IStateManager>(StateManager.Instance);
}
