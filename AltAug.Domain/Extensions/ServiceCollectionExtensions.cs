using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.CraftingStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace AltAug.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection that) => that
        .AddSingleton<IStateManager>(StateManager.Instance)
        .AddKeyedTransient<ICraftingStrategy, AlterationStrategy>(typeof(AlterationStrategy));
}
