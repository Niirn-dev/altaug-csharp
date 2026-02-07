using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Filters;
using AltAug.UI.Elements.Dialogs;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia.Controls;
using Avalonia.Layout;
using FluentAvalonia.UI.Controls;

namespace AltAug.UI.Elements.FilterControls;

internal sealed class RegexFilterControl : IFilterControl<RegexFilter>
{
    private const string ControlTitle = "Regex Filter";

    private readonly RegexLibraryDialog _regexLibrary;

    private readonly Border _root;
    private readonly StackPanel _filterPanel;
    private readonly Panel _headerPanel;
    private readonly Grid _mainGrid;
    private readonly TextBlock _titleText;
    private readonly TextBox _regexTextBox;
    private readonly Button _openLibraryButton;
    private readonly Button _closeButton;

    public bool IsRemoved { get; private set; } = false;
    public IFilterParams Parameters { get => new RegexFilterParameters(_regexTextBox.Text ?? string.Empty); }

    public RegexFilterControl(IStateManager<RegexLibraryStore> regexLibraryManager)
    {
        // Initialize controls
        _regexLibrary = new(regexLibraryManager);

        _root = ControlsLibrary.MakeFilterBorder();

        _filterPanel = new()
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        _headerPanel = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        _mainGrid = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("*, Auto"),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        _titleText = ControlsLibrary.MakeTitleTextBlock(text: ControlTitle);

        _regexTextBox = ControlsLibrary.MakeTextBox(text: string.Empty);

        _openLibraryButton = ControlsLibrary.MakeFixedHeightButton(content: "RegEx Library");
        _openLibraryButton.HorizontalAlignment = HorizontalAlignment.Right;
        _openLibraryButton.Click += async (_, _) =>
        {
            var result = await _regexLibrary.OpenDialogAsync();

            if (result is ContentDialogResult.Primary)
                _regexTextBox.Text = _regexLibrary.RegexString;
        };

        _closeButton = ControlsLibrary.MakeSquareButton(content: "x");
        _closeButton.HorizontalAlignment = HorizontalAlignment.Right;

        // Define layout
        _headerPanel.Children.Add(_titleText);
        _headerPanel.Children.Add(_closeButton);

        _mainGrid.AddControl(_regexTextBox, row: 0, column: 0)
            .AddControl(_openLibraryButton, row: 0, column: 1);

        _filterPanel.Children.Add(_headerPanel);
        _filterPanel.Children.Add(_mainGrid);

        _root.Child = _filterPanel;
    }

    public void Accept(IFilterParams @params)
    {
        if (@params is not RegexFilterParameters regexFilterParams)
            return;

        _regexTextBox.Text = regexFilterParams.RegexString;
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

    public IFilter MakeFilter() => new RegexFilter(_regexTextBox.Text ?? string.Empty);
}
