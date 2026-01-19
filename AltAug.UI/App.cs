using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using AltAug.UI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using MoreLinq;

namespace AltAug.UI;

public sealed class App : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentAvaloniaTheme());

        base.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

internal sealed class MainWindow : AppWindow
{
    public MainWindow()
    {
        Title = "Alt-Aug C# Edition";
        Width = 800;
        Height = 600;
        var rootPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10)
        };

        object[] views =
        [
            new ConfigurationView(),
            new CraftingView(),
            new LoggingView(),
        ];

        views
            .Interleave(Enumerable.Range(0, views.Length - 1).Select(_ => new Separator { Margin = new Thickness(0, 10, 0, 10) }))
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
