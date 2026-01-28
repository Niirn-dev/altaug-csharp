using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Filters;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AltAug.UI.Elements;

internal sealed class OpenSuffixFilterControl : IFilterControl
{
    private const string ControlTitle = "Open Suffix Filter";

    private readonly Border _root;
    private readonly Panel _headerPanel;
    private readonly TextBlock _titleText;
    private readonly Button _closeButton;

    public bool IsRemoved { get; private set; } = false;
    public Type FilterType { get; } = typeof(OpenSuffixFilter);
    public IFilterParams Parameters { get; } = new OpenSuffixFilterParameters();

    public OpenSuffixFilterControl()
    {
        // Initialize controls
        _root = new()
        {
            BorderBrush = Brushes.DimGray,
            BorderThickness = new Thickness(uniformLength: 1),
            CornerRadius = new CornerRadius(uniformRadius: 5),
            Margin = new Thickness(uniformLength: 2),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxWidth = 600,
        };

        _headerPanel = new()
        {
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

        _root.Child = _headerPanel;
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

    public void Accept(IFilterParams @params)
    {
        if (@params is not OpenSuffixFilterParameters)
            // For potential logging
            return;

        // No parameters to apply for this control
        return;
    }

    public IFilter MakeFilter() => new OpenSuffixFilter();
}
