using Microsoft.Extensions.DependencyInjection;

namespace AltAug.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection that) => that
        .AddSingleton(StateManager.Instance);
}
