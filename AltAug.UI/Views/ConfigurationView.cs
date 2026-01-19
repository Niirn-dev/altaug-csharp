using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using MoreLinq;

namespace AltAug.UI.Views;

internal sealed class ConfigurationView : IView
{
    private readonly StackPanel _root;

    public ConfigurationView()
    {
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
        configButtonsStack.Children.Add(new Button
        {
            Content = "TH",
            Height = 126,
            Width = 68,
        });

        Button[] doubledButtons =
        [
            new() { Content = "Alt", Height =  61, Width = 63, Margin = new Thickness(2) },
            new() { Content = "Alch", Height =  61, Width = 63, Margin = new Thickness(2) },
            new() { Content = "Aug", Height =  61, Width = 63, Margin = new Thickness(2) },
            new() { Content = "Scr", Height =  61, Width = 63, Margin = new Thickness(2) },
            new() { Content = "Chs", Height =  61, Width = 63, Margin = new Thickness(2) },
            new() { Content = "Map", Height =  61, Width = 63, Margin = new Thickness(2) },
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
}
