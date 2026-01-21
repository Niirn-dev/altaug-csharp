using AltAug.Domain.Interfaces;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;

namespace AltAug.UI;

internal sealed class App(IServiceProvider serviceProvider) : Application
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override void Initialize()
    {
        Styles.Add(new FluentAvaloniaTheme());

        base.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
           desktop.MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

internal sealed class MainWindow : AppWindow
{
    private readonly IStateManager _stateManager;

    public MainWindow(IEnumerable<IView> views, IStateManager stateManager)
    {
        _stateManager = stateManager;

        Title = "Alt-Aug C# Edition";
        Width = 800;
        Height = 600;
        var rootPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10)
        };

        views.Cast<object>()
            .Interleave(Enumerable.Range(0, views.Count() - 1).Select(_ => new Separator { Margin = new Thickness(0, 10, 0, 10) }))
            .ForEach(item =>
            {
                if (item is IView view)
                {
                    rootPanel.Children.AddView(view);
                }
                else if (item is Control control)
                {
                    rootPanel.Children.Add(control);
                }
            });

        Content = new ScrollViewer
        {
            Content = rootPanel
        };

    }
}
