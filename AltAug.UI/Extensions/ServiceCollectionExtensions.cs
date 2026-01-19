using AltAug.UI.Interfaces;
using AltAug.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AltAug.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterUIServices(this IServiceCollection that) => that
        .AddTransient<IView, ConfigurationView>()
        .AddTransient<IView, CraftingView>()
        .AddTransient<IView, LoggingView>()
        .AddTransient<MainWindow>();
}
