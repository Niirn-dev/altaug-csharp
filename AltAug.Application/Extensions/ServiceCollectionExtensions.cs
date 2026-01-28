using AltAug.Application.CraftingStrategies;
using AltAug.Application.Services;
using AltAug.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AltAug.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection that) => that
        .AddTransient<IAutomationService, AutomationService>()
        .AddTransient<ICraftingService, CraftingService>()
        .AddKeyedTransient<ICraftingStrategy, AlterationStrategy>(typeof(AlterationStrategy))
        .AddKeyedTransient<ICraftingStrategy, AlchemyStrategy>(typeof(AlchemyStrategy))
        .AddKeyedTransient<ICraftingStrategy, ChaosStrategy>(typeof(ChaosStrategy));
}
