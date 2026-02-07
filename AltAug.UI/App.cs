using AltAug.Domain.Helpers;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Elements;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace AltAug.UI;

internal sealed class App(IServiceProvider serviceProvider) : Application
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<App> _logger = serviceProvider.GetRequiredService<ILogger<App>>();

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

        Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (_, args) =>
        {
            _logger.LogError(args.Exception, "Unhandled UI exception occurred: {Exception}.", args.Exception);
            args.Handled = true;
        };

        base.OnFrameworkInitializationCompleted();
    }
}

internal sealed class MainWindow : AppWindow
{
    private readonly IStateManager<AppConfig> _stateManager;

    public MainWindow(IEnumerable<IView> views, IStateManager<AppConfig> stateManager)
    {
        _stateManager = stateManager;

        Title = Constants.ApplicationName;
        Height = _stateManager.State.MainWindowConfig.Height;
        Width = _stateManager.State.MainWindowConfig.Width;
        MinHeight = MainWindowConfig.DefaultHeight;
        MinWidth = MainWindowConfig.DefaultWidth;
        CanResize = true;

        var root = new Grid
        {
            RowDefinitions = RowDefinitions.Parse(
                string.Join(
                    ',',
                    Enumerable.Range(0, (views.Count() - 1) * 2)
                        .Select(_ => "Auto")
                        .Append("*"))),
            Margin = new Thickness(uniformLength: 10)
        };

        views.Cast<object>()
            .Interleave(Enumerable.Range(0, views.Count() - 1).Select(_ => new Separator { Margin = new Thickness(0, vertical: 10) }))
            .ForEach((item, index) =>
            {
                if (item is IView view)
                {
                    root.AddControl(view.GetControl(), row: index, column: 0);
                }
                else if (item is Control control)
                {
                    root.AddControl(control, row: index, column: 0);
                }
            });

        Content = ControlsLibrary.MakeScrollViewer(content: root);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        _stateManager.Update(cfg => cfg with
        {
            MainWindowConfig = new()
            {
                Height = Height - TitleBar.Height + 1,
                Width = Width,
            },
        });

        base.OnClosing(e);
    }
}
