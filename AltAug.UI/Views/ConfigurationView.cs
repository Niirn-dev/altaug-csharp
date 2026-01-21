using AltAug.Domain.Interfaces;
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
    private readonly IStateManager _stateManager;
    private readonly IAutomationService _automationService;

    public ConfigurationView(IStateManager stateManager, IAutomationService automationService)
    {
        _stateManager = stateManager;
        _automationService = automationService;

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
        configButtonsStack.Children.Add(GetItemButton());

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

    private Button GetItemButton()
    {
        var btn = new Button
        {
            Content = "TH",
            Height = 126,
            Width = 68,
        };

        btn.Click += (src, args) =>
        {
            var cursorPosition = _automationService.RecordMousePosition();
            cursorPosition.Match(
                point =>
                {
                    Console.WriteLine($"Recorded mouse cursor position with (x, y): ({point.X}, {point.Y}).");
                    _stateManager.Update(cfg => cfg with
                    {
                        CoordinatesConfig = cfg.CoordinatesConfig with
                        {
                            Item = point,
                        }
                    });
                },
                () => Console.WriteLine("The mouse cursor position wasn't recorded.")
            );
        };

        return btn;
    }
}
