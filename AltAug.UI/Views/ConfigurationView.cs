using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LanguageExt;
using MoreLinq;

namespace AltAug.UI.Views;

internal sealed class ConfigurationView : IView
{
    private const int ConfigurationButtonSingleRowHeight = 61;
    private const int ConfigurationButtonDoubleRowHeight = 126;
    private const int ConfigurationButtonWidth = 63;

    private readonly StackPanel _root;
    private readonly IStateManager _stateManager;
    private readonly IAutomationService _automationService;

    public ConfigurationView(IStateManager stateManager, IAutomationService automationService)
    {
        _stateManager = stateManager;
        _automationService = automationService;

        _root = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0),
        };

        _root.Children.Add(new TextBlock
        {
            Text = "Configuration",
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 5),
        });

        _root.Children.Add(new TextBlock
        {
            Text = "Configure Coordinates:",
        });

        var configButtonsStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(10),
            
        };
        configButtonsStack.Children.Add(
            MakeDoubleRowConfigBtn(
                "avares://AltAug.UI/Assets/bow_item.png",
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Item = point } }
            )
        );

        Button[] doubledButtons =
        [
            MakeSingleRowConfigBtn(
                "avares://AltAug.UI/Assets/alt_orb.png",
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Alteration = point } }
            ),
            MakeSingleRowConfigBtn(
                "avares://AltAug.UI/Assets/alch_orb.png",
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Alchemy = point } }
            ),
            MakeSingleRowConfigBtn(
                "avares://AltAug.UI/Assets/aug_orb.png",
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Augmentation = point } }
            ),
            MakeSingleRowConfigBtn(
                "avares://AltAug.UI/Assets/scour_orb.png",
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Scour = point } }
            ),
            MakeSingleRowConfigBtn(
                "avares://AltAug.UI/Assets/chaos_orb.png",
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Chaos = point } }
            ),
            MakeMapConfigurationBtn(),
        ];
        doubledButtons.Batch(2).Select(buttons =>
            {
                var sp = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0),
                };
                buttons.ForEach(sp.Children.Add);

                return sp;
            })
            .ForEach(configButtonsStack.Children.Add);
        _root.Children.Add(configButtonsStack);

        var delayStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };
        delayStack.Children.Add(new NumericUpDown
        {
            Increment = 0.005m,
            Value = 0.050m,
            Minimum = 0.025m,
            Maximum = 1.0m,
            Width = 120,
            Margin = new Thickness(10, 5),
        });
        delayStack.Children.Add(new TextBlock
        {
            Text = "Set pause after each action (recommended higher than in-game ping)",
            VerticalAlignment = VerticalAlignment.Center,
        });
        _root.Children.Add(delayStack);

        _root.Children.Add(new CheckBox
        {
            Content = "Enable performance logging",
            Margin = new Thickness(10, 5),
        });
    }

    public void AddTo(Controls root)
    {
        root.Add(_root);
    }

    private Button MakeMapConfigurationBtn()
    {
        var uri = new Uri("avares://AltAug.UI/Assets/map.png");
        var bitmap = new Bitmap(AssetLoader.Open(uri));

        var btn = new Button
        {
            Content = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Uniform,
            },
            Height = ConfigurationButtonSingleRowHeight,
            Width = ConfigurationButtonWidth,
            Margin = new Thickness(2),
            Padding = new Thickness(0),
        };

        btn.Click += (src, args) =>
        {
            _automationService.RecordMousePosition().Match(
                point =>
                {
                    Console.WriteLine($"Recorded mouse cursor position with (x, y): ({point.X}, {point.Y}).");
                    _stateManager.Update(cfg => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { MapTopLeft = point } });
                },
                () => Console.WriteLine("The mouse cursor position wasn't recorded.")
            );

            _automationService.RecordMousePosition().Match(
                point =>
                {
                    Console.WriteLine($"Recorded mouse cursor position with (x, y): ({point.X}, {point.Y}).");
                    _stateManager.Update(cfg => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { MapBottomRight = point } });
                },
                () => Console.WriteLine("The mouse cursor position wasn't recorded.")
            );
        };

        return btn;
    }

    private Button MakeDoubleRowConfigBtn(string content, Func<AppConfig, Vec2, AppConfig> configUpdater) => MakeConfigurationButton(
        content,
        height: ConfigurationButtonDoubleRowHeight,
        width: ConfigurationButtonWidth,
        configUpdater);

    private Button MakeSingleRowConfigBtn(string content, Func<AppConfig, Vec2, AppConfig> configUpdater) => MakeConfigurationButton(
        content,
        height: ConfigurationButtonSingleRowHeight,
        width: ConfigurationButtonWidth,
        configUpdater);

    private Button MakeConfigurationButton(string content, int height, int width, Func<AppConfig, Vec2, AppConfig> configUpdater)
    {
        var uri = new Uri(content);
        var bitmap = new Bitmap(AssetLoader.Open(uri));

        var btn = new Button
        {
            Content = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Uniform,
            },
            Height = height,
            Width = width,
            Margin = new Thickness(2),
            Padding = new Thickness(0),
        };

        btn.Click += (src, args) =>
        {
            var cursorPosition = _automationService.RecordMousePosition();
            cursorPosition.Match(
                point =>
                {
                    Console.WriteLine($"Recorded mouse cursor position with (x, y): ({point.X}, {point.Y}).");
                    _stateManager.Update(cfg => configUpdater(cfg, point));
                },
                () => Console.WriteLine("The mouse cursor position wasn't recorded.")
            );
        };

        return btn;
    }
}
