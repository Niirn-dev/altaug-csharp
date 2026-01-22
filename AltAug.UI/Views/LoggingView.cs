using AltAug.UI.Interfaces;
using AltAug.UI.Logging;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Views;

public sealed class LoggingView(ILoggerFactory loggerFactory) : IView
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    public void AddTo(Controls root)
    {
        var logTextBox = new TextBox()
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            Height = 150,
            Text = string.Empty,
        };

        var clearButton = new Button
        {
            Content = "Clear Log",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 5, 0, 0)
        };
        clearButton.Click += (s, e) => logTextBox.Text = string.Empty;

        root.Add(new TextBlock
        {
            Text = "Logging",
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        });
        root.Add(logTextBox);
        root.Add(clearButton);

        _loggerFactory.AddProvider(new TextBoxLoggerProvider(logTextBox));
    }
}
