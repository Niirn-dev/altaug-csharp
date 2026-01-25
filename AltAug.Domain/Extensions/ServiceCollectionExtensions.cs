using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.CraftingStrategies;
using AltAug.Domain.Models.StateManagers;
using Microsoft.Extensions.DependencyInjection;

namespace AltAug.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection that) => that
        .AddSingleton<IStateManager<AppConfig>>(ApplicationStateManager.Instance)
        .AddSingleton<IStateManager<RegexLibraryStore>>(RegexLibraryStateManager.Instance)
        .AddKeyedTransient<ICraftingStrategy, AlterationStrategy>(typeof(AlterationStrategy));
}
