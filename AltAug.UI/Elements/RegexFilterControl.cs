using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Filters;
using AltAug.UI.Elements.Dialogs;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;

namespace AltAug.UI.Elements;

internal sealed class RegexFilterControl : IFilterControl
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

    public RegexFilterControl(IStateManager<RegexLibraryStore> regexLibraryManager)
    {
        // Initialize controls
        _regexLibrary = new(regexLibraryManager);

        _root = new()
        {
            BorderBrush = Brushes.DimGray,
            BorderThickness = new Thickness(uniformLength: 1),
            CornerRadius = new CornerRadius(uniformRadius: 5),
            Margin = new Thickness(uniformLength: 2),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxWidth = 600,
        };

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

        _titleText = new()
        {
            Text = ControlTitle,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(uniformLength: 2),
        };

        _regexTextBox = new()
        {
            Height = 30,
            MinWidth = 300,
            Margin = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        _openLibraryButton = new()
        {
            Content = "RegEx Library",
            Height = 30,
            Margin = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        _openLibraryButton.Click += async (_, _) =>
        {
            var result = await _regexLibrary.OpenDialogAsync();

            if (result is ContentDialogResult.Primary)
                _regexTextBox.Text = _regexLibrary.RegexString;
        };

        _closeButton = new()
        {
            Content = "x",
            Height = 30,
            Width = 30,
            Margin = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        // Define layout
        _headerPanel.Children.Add(_titleText);
        _headerPanel.Children.Add(_closeButton);

        _mainGrid.AddControl(_regexTextBox, row: 0, column: 0)
            .AddControl(_openLibraryButton, row: 0, column: 1);

        _filterPanel.Children.Add(_headerPanel);
        _filterPanel.Children.Add(new Separator { Margin = new Thickness(0, vertical: 4) });
        _filterPanel.Children.Add(_mainGrid);

        _root.Child = _filterPanel;
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
