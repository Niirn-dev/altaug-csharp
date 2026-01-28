using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Filters;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AltAug.UI.Elements;

internal sealed class OpenPrefixFilterControl : IFilterControl
{
    private const string ControlTitle = "Open Prefix Filter";

    private readonly Border _root;
    private readonly Panel _headerPanel;
    private readonly TextBlock _titleText;
    private readonly Button _closeButton;

    public bool IsRemoved { get; private set; } = false;
    public Type FilterType { get; } = typeof(OpenPrefixFilter);
    public IFilterParams Parameters { get; } = new OpenPrefixFilterParameters();

    public OpenPrefixFilterControl()
    {
        // Initialize controls
        _root = ControlsLibrary.MakeFilterBorder();

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

    public void Accept(IFilterParams @params)
    {
        if (@params is not OpenPrefixFilterParameters)
            // For potential logging
            return;

        // No parameters to apply for this control
        return;
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

    public IFilter MakeFilter() => new OpenPrefixFilter();
}
