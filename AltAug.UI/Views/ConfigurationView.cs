using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Elements;
using AltAug.UI.Elements.Dialogs;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using FluentAvalonia.Core;
using LanguageExt;
using Microsoft.Extensions.Logging;

using static LanguageExt.Prelude;
using static MoreLinq.Extensions.BatchExtension;
using static MoreLinq.Extensions.ForEachExtension;

namespace AltAug.UI.Views;

internal sealed partial class ConfigurationView : IView
{
    private const string ViewTitle = "Configuration";
    private const string AutoGuiPauseHintText = "Set pause after each action (recommended higher than in-game ping)";
    private const string CraftingDelayHintText = "Delay before crafting starts to swap focus to PoE window";
    private const string ScreenResolutionHintText = "Screen Resolution (not PoE resolution)";
    private const int ConfigurationButtonSingleRowHeight = 61;
    private const int ConfigurationButtonDoubleRowHeight = 126;
    private const int ConfigurationButtonWidth = 63;

    private static readonly IReadOnlyDictionary<string, Vec2> ScreenResolutionLookup = new Dictionary<string, Vec2>()
    {
        [ "800x600" ] = new Vec2(800, 600),
        [ "1024x768" ] = new Vec2(1024, 768),
        [ "1280x720" ] = new Vec2(1280, 720),
        [ "1280x1024" ] = new Vec2(1280, 1024),
        [ "1366x768" ] = new Vec2(1366, 768),
        [ "1600x900" ] = new Vec2(1600, 900),
        [ "1920x1080" ] = new Vec2(1920, 1080),
        [ "2560x1080" ] = new Vec2(2560, 1080),
        [ "2560x1440" ] = new Vec2(2560, 1440),
        [ "3440x1440" ] = new Vec2(3440, 1440),
        [ "3840x1600" ] = new Vec2(3840, 1600),
        [ "3840x2160" ] = new Vec2(3840, 2160),
        [ "5120x2160" ] = new Vec2(5120, 2160),
        [ "5120x2880" ] = new Vec2(5120, 2880),
        [ "6016x3384" ] = new Vec2(6016, 3384),
        [ "7680x4320" ] = new Vec2(7680, 4320),
    };
    private static readonly int DefaultScreenResolutionComboBoxIndex = ScreenResolutionLookup.IndexOf("1920x1080");

    private readonly IStateManager<AppConfig> _appManager;
    private readonly IAutomationService _automationService;
    private readonly ILogger<ConfigurationView> _logger;

    private readonly Expander _root;
    private readonly StackPanel _mainPanel;

    private readonly Grid _automationConfigGrid;
    private readonly TextBlock _autoGuiPauseHintTextBlock;
    private readonly TextBlock _craftingDelayHintTextBlock;
    private readonly TextBlock _screenResolutionHintTextBlock;
    private readonly NumericUpDown _autoGuiPauseUpDown;
    private readonly NumericUpDown _craftingDelayUpDown;
    private readonly ComboBox _screenResolutionComboBox;

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
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Item = point } },
                hoverTarget: "Currency tab's item slot"
            )
        );

        Button[] doubledButtons =
        [
            MakeSingleRowConfigBtn(
                AssetLibrary.GetAlterationOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Alteration = point } },
                hoverTarget: "Alteration orb"
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetAlchemyOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Alchemy = point } },
                hoverTarget: "Alchemy orb"
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetAugmentationOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Augmentation = point } },
                hoverTarget: "Augmentation orb"
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetScouringOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Scour = point } },
                hoverTarget: "Scouring orb"
            ),
            MakeSingleRowConfigBtn(
                AssetLibrary.GetChaosOrbBitmap(),
                (cfg, point) => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { Chaos = point } },
                hoverTarget: "Chaos orb"
            ),
            MakeMapConfigurationBtn(),
        ];

        _automationConfigGrid = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, *"),
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, Auto"),
        };

        _autoGuiPauseHintTextBlock = ControlsLibrary.MakeSingleLineTextBlock(text: AutoGuiPauseHintText);

        _autoGuiPauseUpDown = new()
        {
            Increment = 0.005m,
            Value = (decimal)_appManager.State.AutomationConfig.AutoGuiPause,
            Minimum = 0.025m,
            Maximum = 1.0m,
            Width = 120,
            Margin = new Thickness(uniformLength: 4),
            HorizontalAlignment = HorizontalAlignment.Left,
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
            Margin = new Thickness(uniformLength: 4),
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        _craftingDelayUpDown.ValueChanged += (_, args) => _appManager.Update(
            cfg => cfg with
            {
                AutomationConfig = cfg.AutomationConfig with
                {
                    CraftingStartDelay = (double?)args.NewValue ?? AutomationConfig.DefaultCraftingStartDelay,
                }
            });

        _screenResolutionHintTextBlock = ControlsLibrary.MakeSingleLineTextBlock(text: ScreenResolutionHintText);

        _screenResolutionComboBox = ControlsLibrary.MakeAutoWidthComboBox();
        ScreenResolutionLookup.Keys.ForEach(k => _screenResolutionComboBox.Items.Add(k));

        _screenResolutionComboBox.SelectedIndex = ScreenResolutionLookup.Values
            .Index()
            .Find(p => p.Item == _appManager.State.AutomationConfig.ScreenResolution)
            .Map(s => s.Index)
            .IfNone(DefaultScreenResolutionComboBoxIndex);

        _screenResolutionComboBox.SelectionChanged += (_, _) => _appManager.Update(
            cfg => cfg with
            {
                AutomationConfig = cfg.AutomationConfig with
                {
                    ScreenResolution = ScreenResolutionLookup
                        .TryGetValue(_screenResolutionComboBox.SelectedValueOrDefault(string.Empty))
                        .IfNone(AutomationConfig.DefaultScreenResolution),
                }
            });

        // Define layout
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

        _automationConfigGrid.AddControl(_autoGuiPauseUpDown, row: 0, column: 0)
            .AddControl(_autoGuiPauseHintTextBlock, row: 0, column: 1)
            .AddControl(_craftingDelayUpDown, row: 1, column: 0)
            .AddControl(_craftingDelayHintTextBlock, row: 1, column: 1)
            .AddControl(_screenResolutionComboBox, row: 2, column: 0)
            .AddControl(_screenResolutionHintTextBlock, row: 2, column: 1);

        _mainPanel.Children.Add(configButtonsStack);
        _mainPanel.Children.Add(_automationConfigGrid);

        var header = ControlsLibrary.MakeTitleTextBlock(text: ViewTitle);
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

        btn.Click += async (src, args) =>
        {
            var dialog = new MousePositionRecordingDialog(_automationService);
            var cursorPosition = await dialog.ShowDialogAsync(hoverTarget: "top-left corner of the first inventory slot");
            cursorPosition.Match(
                point =>
                {
                    LogInfoCursorPosition(point.X, point.Y);
                    _appManager.Update(cfg => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { InventorySlotTopLeft = point } });
                },
                () => LogInfoCursorPositionNotRecorded()
            );

            if (cursorPosition.IsNone)
                return;

            cursorPosition = await dialog.ShowDialogAsync(hoverTarget: "bottom-right corner of the first inventory slot");
            cursorPosition.Match(
                point =>
                {
                    LogInfoCursorPosition(point.X, point.Y);
                    _appManager.Update(cfg => cfg with { CoordinatesConfig = cfg.CoordinatesConfig with { InventorySlotBottomRight = point } });
                },
                () => LogInfoCursorPositionNotRecorded()
            );
        };

        return btn;
    }

    private Button MakeDoubleRowConfigBtn(Bitmap imageSource, Func<AppConfig, Vec2, AppConfig> configUpdater, string hoverTarget) => MakeConfigurationButton(
        imageSource,
        height: ConfigurationButtonDoubleRowHeight,
        width: ConfigurationButtonWidth,
        configUpdater,
        hoverTarget);

    private Button MakeSingleRowConfigBtn(Bitmap imageSource, Func<AppConfig, Vec2, AppConfig> configUpdater, string hoverTarget) => MakeConfigurationButton(
        imageSource,
        height: ConfigurationButtonSingleRowHeight,
        width: ConfigurationButtonWidth,
        configUpdater,
        hoverTarget);

    private Button MakeConfigurationButton(Bitmap imageSource, int height, int width, Func<AppConfig, Vec2, AppConfig> configUpdater, string hoverTarget)
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

        btn.Click += async (src, args) =>
        {
            var dialog = new MousePositionRecordingDialog(_automationService);
            var cursorPosition = await dialog.ShowDialogAsync(hoverTarget);
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
