using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Elements;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LanguageExt;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace AltAug.UI.Views;

internal sealed partial class ConfigurationView : IView
{
    private const string ViewTitle = "Configuration";
    private const string AutoGuiPauseHintText = "Set pause after each action (recommended higher than in-game ping)";
    private const string CraftingDelayHintText = "Delay before crafting starts to swap focus to PoE window";
    private const int ConfigurationButtonSingleRowHeight = 61;
    private const int ConfigurationButtonDoubleRowHeight = 126;
    private const int ConfigurationButtonWidth = 63;

    private readonly IStateManager<AppConfig> _appManager;
    private readonly IAutomationService _automationService;
    private readonly ILogger<ConfigurationView> _logger;

    private readonly Expander _root;
    private readonly StackPanel _mainPanel;

    private readonly Grid _automationConfigGrid;
    private readonly TextBlock _autoGuiPauseHintTextBlock;
    private readonly TextBlock _craftingDelayHintTextBlock;
    private readonly NumericUpDown _autoGuiPauseUpDown;
    private readonly NumericUpDown _craftingDelayUpDown;

    public ConfigurationView(IStateManager<AppConfig> stateManager, IAutomationService automationService, ILogger<ConfigurationView> logger)
    {
        _appManager = stateManager;
        _automationService = automationService;
        _logger = logger;

        _mainPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0),
        };

        var configButtonsStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(uniformLength: 4),
            
        };
        configButtonsStack.Children.Add(
            MakeDoubleRowConfigBtn(
                AssetLibrary.GetBowItemBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Item = point } }
            )
        );

        Button[] doubledButtons =
        [
            MakeSingleRowConfigBtn(
                AssetLibrary.GetAlterationOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Alteration = point } }
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetAlchemyOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Alchemy = point } }
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetAugmentationOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Augmentation = point } }
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetScouringOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Scour = point } }
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetChaosOrbBitmap(),
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
        _mainPanel.Children.Add(configButtonsStack);

        _automationConfigGrid = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, *"),
            RowDefinitions = RowDefinitions.Parse("Auto, Auto"),
        };

        _autoGuiPauseHintTextBlock = ControlsLibrary.MakeSingleLineTextBlock(text: AutoGuiPauseHintText);

        _autoGuiPauseUpDown = new()
        {
            Increment = 0.005m,
            Value = (decimal)_appManager.State.AutomationConfig.AutoGuiPause,
            Minimum = 0.025m,
            Maximum = 1.0m,
            Width = 120,
            Margin = new Thickness(10, 5),
        };
        _autoGuiPauseUpDown.ValueChanged += (src, args) => _appManager.Update(
            cfg => cfg with
            {
                AutomationConfig = cfg.AutomationConfig with
                {
                    AutoGuiPause = (double?)args.NewValue ?? AutomationConfig.DefaultAutoGuiPause,
                }
            });

        _craftingDelayHintTextBlock = ControlsLibrary.MakeSingleLineTextBlock(text: CraftingDelayHintText);

        _craftingDelayUpDown = new()
        {
            Increment = 0.1m,
            Value = (decimal)_appManager.State.AutomationConfig.CraftingStartDelay,
            Minimum = 0.5m,
            Maximum = 2.5m,
            Width = 120,
            Margin = new Thickness(10, 5),
        };
        _craftingDelayUpDown.ValueChanged += (_, args) => _appManager.Update(
            cfg => cfg with
            {
                AutomationConfig = cfg.AutomationConfig with
                {
                    CraftingStartDelay = (double?)args.NewValue ?? AutomationConfig.DefaultCraftingStartDelay,
                }
            });

        // Define layout
        _automationConfigGrid.AddControl(_autoGuiPauseUpDown, row: 0, column: 0)
            .AddControl(_autoGuiPauseHintTextBlock, row: 0, column: 1)
            .AddControl(_craftingDelayUpDown, row: 1, column: 0)
            .AddControl(_craftingDelayHintTextBlock, row: 1, column: 1);

        _mainPanel.Children.Add(_automationConfigGrid);

        var header = ControlsLibrary.MakeTitleTextBlock(text: ViewTitle);
        header.Margin = new Thickness(uniformLength: 0);
        _root = new()
        {
            Header = header,
            IsExpanded = false,
            ExpandDirection = ExpandDirection.Down,
            Content = _mainPanel,
        };
    }

    public Control GetControl() => _root;

    private Button MakeMapConfigurationBtn()
    {
        var btn = new Button
        {
            Content = new Image
            {
                Source = AssetLibrary.GetMapItemBitmap(),
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
                    LogInfoCursorPosition(point.X, point.Y);
                    _appManager.Update(cfg => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { MapTopLeft = point } });
                },
                () => LogInfoCursorPositionNotRecorded()
            );

            _automationService.RecordMousePosition().Match(
                point =>
                {
                    LogInfoCursorPosition(point.X, point.Y);
                    _appManager.Update(cfg => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { MapBottomRight = point } });
                },
                () => LogInfoCursorPositionNotRecorded()
            );
        };

        return btn;
    }

    private Button MakeDoubleRowConfigBtn(Bitmap imageSource, Func<AppConfig, Vec2, AppConfig> configUpdater) => MakeConfigurationButton(
        imageSource,
        height: ConfigurationButtonDoubleRowHeight,
        width: ConfigurationButtonWidth,
        configUpdater);

    private Button MakeSingleRowConfigBtn(Bitmap imageSource, Func<AppConfig, Vec2, AppConfig> configUpdater) => MakeConfigurationButton(
        imageSource,
        height: ConfigurationButtonSingleRowHeight,
        width: ConfigurationButtonWidth,
        configUpdater);

    private Button MakeConfigurationButton(Bitmap imageSource, int height, int width, Func<AppConfig, Vec2, AppConfig> configUpdater)
    {
        var btn = new Button
        {
            Content = new Image
            {
                Source = imageSource,
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
                    LogInfoCursorPosition(point.X, point.Y);
                    _appManager.Update(cfg => configUpdater(cfg, point));
                },
                () => LogInfoCursorPositionNotRecorded()
            );
        };

        return btn;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Recorded mouse cursor position with ({X}, {Y}).")]
    private partial void LogInfoCursorPosition(double x, double y);

    [LoggerMessage(Level = LogLevel.Information, Message = "The mouse cursor position wasn't recorded.")]
    private partial void LogInfoCursorPositionNotRecorded();
}
