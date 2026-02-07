using System.Reflection;
using AltAug.Domain.Extensions;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using AltAug.UI.Elements;
using AltAug.UI.Elements.Dialogs;
using AltAug.UI.Extensions;
using AltAug.UI.Helpers;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace AltAug.UI.Views;

internal sealed class CraftingView : IView
{
    private const string ViewTitle = "Crafting";

    private readonly IServiceProvider _serviceProvider;
    private readonly IFilterControlFactory _filterControlFactory;
    private readonly IStateManager<AppConfig> _appManager;
    private readonly CraftingProgressDialog _craftingDialog;

    private readonly StackPanel _root;
    private readonly ComboBox _itemLocationComboBox;
    private readonly ComboBox _craftingStrategyComboBox;
    private readonly NumericUpDown _itemCountUpDown;
    private readonly TextBlock _itemCountText;
    private readonly NumericUpDown _currencyUsedUpDown;
    private readonly TextBlock _currencyUsedText;

    private readonly StackPanel _selectedFilterPanel;
    private readonly List<IFilterControl> _selectedFilterControls = [];

    private readonly ComboBox _filterComboBox;
    private readonly Button _addFilterButton;
    private readonly Button _startCraftButton;

    private readonly Type[] _craftingStrategyTypes;
    private readonly Type[] _filterTypes;

    private Type SelectedCraftingStrategyType { get => _craftingStrategyTypes[_craftingStrategyComboBox.SelectedIndex]; }
    private Type SelectedFilterType { get => _filterTypes[_filterComboBox.SelectedIndex]; }

    public CraftingView(
        IServiceProvider serviceProvider,
        IAutomationService automationService,
        ICraftingService craftingService,
        IFilterControlFactory filterControlFactory,
        IStateManager<AppConfig> appManager,
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _filterControlFactory = filterControlFactory;
        _appManager = appManager;
        _craftingDialog = new(appManager, automationService, craftingService, loggerFactory);

        // Initialize controls
        _root = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0),
        };

        _itemLocationComboBox = ControlsLibrary.MakeComboBox();
        Enum.GetNames<ItemLocation>()
            .ForEach(l => _itemLocationComboBox.Items.Add(l));
        _itemLocationComboBox.SelectedIndex = _appManager.State.CraftingConfig.ItemLocationIndex;

        _craftingStrategyComboBox = ControlsLibrary.MakeComboBox();
        _craftingStrategyTypes = [.. AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => typeof(ICraftingStrategy).IsAssignableFrom(t)
                && t.IsClass
                && !t.IsAbstract)];
        _craftingStrategyTypes.ForEach(t => _craftingStrategyComboBox.Items.Add(t.Name));
        _craftingStrategyComboBox.SelectedIndex = _appManager.State.CraftingConfig.CraftingStrategyIndex;

        _itemCountUpDown = ControlsLibrary.MakeIntUpDown(value: _appManager.State.CraftingConfig.ItemsToCraft);

        _itemCountText = ControlsLibrary.MakeSingleLineTextBlock(text: "Items to craft");

        _currencyUsedUpDown = ControlsLibrary.MakeIntUpDown(value: _appManager.State.CraftingConfig.CurrencyToUseCount);

        _currencyUsedText = ControlsLibrary.MakeSingleLineTextBlock(text: "Currency to use");

        _selectedFilterPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(uniformLength: 0),
        };
        _appManager.State.CraftingConfig.Filters
            .ForEach(f => SerializationHelper.FilterSerializedNameToType.TryGetValue(f.FilterTypeName)
                .IfSome(filterType =>
                {
                    var filterControl = _filterControlFactory.Create(filterType);
                    filterControl.Accept(f.Parameters);

                    filterControl.AddTo(_selectedFilterPanel.Children);
                    _selectedFilterControls.Add(filterControl);
                }));

        _filterComboBox = ControlsLibrary.MakeComboBox();
        _filterTypes = [.. Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFilterControl<>))
                .Select(i => i.GetGenericArguments()[0])
            )];
        _filterTypes.ForEach(t => _filterComboBox.Items.Add(t.Name));
        _filterComboBox.SelectedIndex = 0;

        _addFilterButton = ControlsLibrary.MakeSquareButton(content: "+");
        _addFilterButton.HorizontalAlignment = HorizontalAlignment.Right;
        _addFilterButton.Click += (_, _) =>
        {
            var filterControl = _filterControlFactory.Create(SelectedFilterType);

            filterControl.AddTo(_selectedFilterPanel.Children);
            _selectedFilterControls.Add(filterControl);

            _selectedFilterControls.RemoveAll(f => f.IsRemoved);
        };

        _startCraftButton = ControlsLibrary.MakeFixedHeightButton(content: "Start crafting");
        _startCraftButton.HorizontalAlignment = HorizontalAlignment.Right;
        _startCraftButton.Click += async (_, _) =>
        {
            var strategy = _serviceProvider.GetRequiredKeyedService<ICraftingStrategy>(SelectedCraftingStrategyType);

            var filters = _selectedFilterControls
                .Where(c => !c.IsRemoved)
                .Select(c => c.MakeFilter())
                .ToList();
            var itemLocation = ((string?)_itemLocationComboBox.SelectedValue)
                .ToOpt()
                .Match(
                    s => s.ParseEnum<ItemLocation>(),
                    () => Option<ItemLocation>.None
                )
                .IfNone(ItemLocation.CurrencyTab);
            var inventoryPosition = itemLocation switch
            {
                ItemLocation.Inventory => 0,
                _ => Option<int>.None,
            };

            var itemsCount = (int?)_itemCountUpDown.Value ?? CraftingConfig.DefaultItemsToCraft;
            var maxAttempts = (int?)_currencyUsedUpDown.Value ?? CraftingConfig.DefaultCurrencyToUseCount;
            var locationParams = new ItemLocationParams(itemLocation, inventoryPosition, Option<Vec2>.None);

            await _craftingDialog.OpenDialogAsync(strategy, filters, locationParams, itemsCount, maxAttempts);
        };


        _root.Unloaded += (_, _) =>
        {
            var selectedFilterState = _selectedFilterControls
                .Where(f => !f.IsRemoved)
                .Select(f => (f.GetSerializedName(), f.Parameters))
                .ToArray();

            _appManager.Update(cfg => cfg with
            {
                CraftingConfig = cfg.CraftingConfig with
                {
                    ItemLocationIndex = _itemLocationComboBox.SelectedIndex,
                    CraftingStrategyIndex = _craftingStrategyComboBox.SelectedIndex,
                    ItemsToCraft = (int?)_itemCountUpDown.Value ?? CraftingConfig.DefaultItemsToCraft,
                    CurrencyToUseCount = (int?)_currencyUsedUpDown.Value ?? CraftingConfig.DefaultCurrencyToUseCount,
                    Filters = selectedFilterState,
                }
            });
        };

        // Define layout
        _root.Children.Add(ControlsLibrary.MakeTitleTextBlock(text: ViewTitle));

        _root.Children.Add(new Border
        {
            BorderBrush = Brushes.DimGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(0, 0, 0, bottom: 5),
            Child = MakeStrategySelectorGrid(), 
        });

        _root.Children.Add(_selectedFilterPanel);

        _root.Children.Add(new Border
        {
            BorderBrush = Brushes.DimGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(0, top: 5, 0, 0),
            Child = MakeFilterAdderGrid(),
        });
    }

    public Control GetControl() => _root;

    private Grid MakeStrategySelectorGrid()
    {
        var grid = new Grid
        {
            ShowGridLines = false,
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, Auto"),
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, Auto, Auto"),
        };

        grid.AddControl(_itemLocationComboBox, row: 0, column: 0)
            .AddControl(_craftingStrategyComboBox, row: 1, column: 0)
            .AddControl(_itemCountUpDown, row: 0, column: 1)
            .AddControl(_itemCountText, row: 0, column: 2)
            .AddControl(_currencyUsedUpDown, row: 1, column: 1)
            .AddControl(_currencyUsedText, row: 1, column: 2);

        return grid;
    }

    private Grid MakeFilterAdderGrid()
    {
        var grid = new Grid
        {
            ShowGridLines = false,
            ColumnDefinitions = ColumnDefinitions.Parse("Auto, Auto, *, Auto"),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        grid.AddControl(_filterComboBox, row: 0, column: 0)
            .AddControl(_addFilterButton, row: 0, column: 1)
            .AddControl(_startCraftButton, row: 0, column: 2);

        return grid;
    }
}
