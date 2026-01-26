using AltAug.Domain.Extensions;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using AltAug.UI.Elements;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;

namespace AltAug.UI.Views;

internal sealed class CraftingView : IView
{
    private const string ViewTitle = "Crafting";
    private const int DefaultItemsToCraft = 1;
    private const int DefaultCurrencyCountToUse = 20;

    private readonly IServiceProvider _serviceProvider;
    private readonly ICraftingService _craftingService;
    private readonly IFilterControlFactory _filterControlFactory;

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

    public CraftingView(IServiceProvider serviceProvider, ICraftingService craftingService, IFilterControlFactory filterControlFactory)
    {
        _serviceProvider = serviceProvider;
        _craftingService = craftingService;
        _filterControlFactory = filterControlFactory;

        // Initialize controls
        _root = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0),
        };

        _itemLocationComboBox = ControlsLibrary.MakeComboBox();
        Enum.GetNames<ItemLocation>()
            .ForEach(l => _itemLocationComboBox.Items.Add(l));
        _itemLocationComboBox.SelectedIndex = 0;

        _craftingStrategyComboBox = ControlsLibrary.MakeComboBox();
        _craftingStrategyTypes = [.. AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => typeof(ICraftingStrategy).IsAssignableFrom(t)
                && t.IsClass
                && !t.IsAbstract)];
        _craftingStrategyTypes.ForEach(t => _craftingStrategyComboBox.Items.Add(t.Name));
        _craftingStrategyComboBox.SelectedIndex = 0;

        _itemCountUpDown = new NumericUpDown
        {
            Value = DefaultItemsToCraft,
            Height = 30,
            Width = 125,
            Margin = new Thickness(4),
            Increment = 1,
            Minimum = 1,
            FormatString = "0",
        };

        _itemCountText = ControlsLibrary.MakeTextBlock(content: "Items to craft");

        _currencyUsedUpDown = ControlsLibrary.MakeIntUpDown(value: DefaultCurrencyCountToUse);

        _currencyUsedText = ControlsLibrary.MakeTextBlock(content: "Currency to use");

        _selectedFilterPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(uniformLength: 0),
        };

        _filterComboBox = ControlsLibrary.MakeComboBox();
        _filterTypes = [.. AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t => typeof(IFilter).IsAssignableFrom(t)
                && t.IsClass
                && !t.IsAbstract)];
        _filterTypes.ForEach(t => _filterComboBox.Items.Add(t.Name));
        _filterComboBox.SelectedIndex = 0;

        _addFilterButton = ControlsLibrary.MakeSquareButton(content: "x");
        _addFilterButton.HorizontalAlignment = HorizontalAlignment.Right;
        _addFilterButton.Click += (_, _) =>
        {
            var filterControl = _filterControlFactory.Create(_filterTypes[_filterComboBox.SelectedIndex]);

            filterControl.AddTo(_selectedFilterPanel.Children);
            _selectedFilterControls.Add(filterControl);

            _selectedFilterControls.RemoveAll(f => f.IsRemoved);
        };

        _startCraftButton = ControlsLibrary.MakeFixedHeightButton(content: "Start crafting");
        _startCraftButton.HorizontalAlignment = HorizontalAlignment.Right;
        _startCraftButton.Padding = new Thickness(horizontal: 4, vertical: 0);
        _startCraftButton.Click += (_, _) =>
        {
            var strategy = _serviceProvider.GetRequiredKeyedService<ICraftingStrategy>(_craftingStrategyTypes[_craftingStrategyComboBox.SelectedIndex]);

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
                ItemLocation.Inventory => 1,
                _ => Option<int>.None,
            };

            var itemsCount = _itemCountUpDown.Value.HasValue
                ? (int)_itemCountUpDown.Value.Value
                : DefaultItemsToCraft;

            var maxAttempts = _currencyUsedUpDown.Value.HasValue
                ? (int)_currencyUsedUpDown.Value.Value
                : DefaultCurrencyCountToUse;

            var locationParams = new ItemLocationParams(itemLocation, inventoryPosition, Option<Vec2>.None);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            _craftingService.CraftItems(strategy, filters, locationParams, itemsCount, maxAttempts);
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

    public void AddTo(Controls root) => root.Add(_root);

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
