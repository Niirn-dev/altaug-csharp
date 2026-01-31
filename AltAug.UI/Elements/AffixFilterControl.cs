using AltAug.Domain.Extensions;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Filters;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia.Controls;
using Avalonia.Layout;
using LanguageExt;

namespace AltAug.UI.Elements;

internal sealed class AffixFilterControl : IFilterControl
{
    private const string ControlTitle = "Affix Filter";
    private const string InputHeader_Name = "Affix Name";
    private const string InputHeader_Description = "Affix Description";
    private const string InputHeader_Tier = "Worst Tier";
    private const int DefaultMaxTierValue = 1;

    private readonly Border _root;
    private readonly Grid _mainGrid;
    private readonly TextBlock _titleText;
    private readonly Button _closeButton;

    private readonly TextBlock _inputHeaderTextBlock_Name;
    private readonly TextBlock _inputHeaderTextBlock_Description;
    private readonly CheckBox _inputHeaderTierEnabledCheckBox;

    private readonly TextBox _inputTextBox_Name;
    private readonly TextBox _inputTextBox_Description;
    private readonly NumericUpDown _inputUpDown_Tier;

    public bool IsRemoved { get; private set; } = false;
    public Type FilterType { get; } = typeof(AffixFilter);
    public IFilterParams Parameters
    {
        get => new AffixFilterParameters(
            NameFilter: _inputTextBox_Name.Text ?? string.Empty,
            DescriptionFilter: _inputTextBox_Description.Text ?? string.Empty,
            MaxTierFilter: (int?)_inputUpDown_Tier.Value ?? DefaultMaxTierValue,
            IsTierFilterEnabled: _inputHeaderTierEnabledCheckBox.IsChecked is true);
    }

    private Option<int> SelectedMaxTier
    {
        get
        {
            if (_inputHeaderTierEnabledCheckBox.IsChecked is not true)
                return Option<int>.None;

            return ((int?)_inputUpDown_Tier.Value).ToOption();
        }
    }

    public AffixFilterControl()
    {
        // Initialize controls
        _root = ControlsLibrary.MakeFilterBorder();

        _mainGrid = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("*, 2*, Auto"),
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, Auto"),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        _titleText = ControlsLibrary.MakeTitleTextBlock(text: ControlTitle);

        _closeButton = ControlsLibrary.MakeSquareButton(content: "x");
        _closeButton.HorizontalAlignment = HorizontalAlignment.Right;

        _inputHeaderTextBlock_Name = ControlsLibrary.MakeSingleLineTextBlock(text: InputHeader_Name);
        _inputHeaderTextBlock_Description = ControlsLibrary.MakeSingleLineTextBlock(text: InputHeader_Description);
        _inputHeaderTierEnabledCheckBox = ControlsLibrary.MakeCheckBox(content: InputHeader_Tier);
        _inputHeaderTierEnabledCheckBox.IsChecked = true;

        _inputTextBox_Name = ControlsLibrary.MakeTextBox(text: string.Empty);
        _inputTextBox_Description = ControlsLibrary.MakeTextBox(text: string.Empty);
        _inputUpDown_Tier = ControlsLibrary.MakeIntUpDown(value: 1);

        // Define layout
        _mainGrid
            .AddControl(_titleText, row: 0, column: 0)
            .AddControl(_closeButton, row: 0, column: 2)
            .AddControl(_inputHeaderTextBlock_Name, row: 1, column: 0)
            .AddControl(_inputHeaderTextBlock_Description, row: 1, column: 1)
            .AddControl(_inputHeaderTierEnabledCheckBox, row: 1, column: 2)
            .AddControl(_inputTextBox_Name, row: 2, column: 0)
            .AddControl(_inputTextBox_Description, row: 2, column: 1)
            .AddControl(_inputUpDown_Tier, row: 2, column: 2);

        _root.Child = _mainGrid;
    }

    public void Accept(IFilterParams @params)
    {
        if (@params is not AffixFilterParameters parameters)
            return;

        _inputTextBox_Name.Text = parameters.NameFilter;
        _inputTextBox_Description.Text = parameters.DescriptionFilter;
        _inputUpDown_Tier.Value = parameters.MaxTierFilter;
        _inputHeaderTierEnabledCheckBox.IsChecked = parameters.IsTierFilterEnabled;
    }

    public void AddTo(Controls controls)
    {
        _closeButton.Click += (_, _) =>
        {
            IsRemoved = true;
            controls.Remove(_root);
        };

        controls.Add(_root);
    }

    public IFilter MakeFilter() => new AffixFilter(
        nameFilter: _inputTextBox_Name.Text.ToOpt(treatEmptyAsNone: true),
        descriptionFilter: _inputTextBox_Description.Text.ToOpt(treatEmptyAsNone: true),
        maxTierFilter: SelectedMaxTier);
}
