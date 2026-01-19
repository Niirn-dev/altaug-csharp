using System;
using System.Collections.Generic;
using System.Text;
using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AltAug.UI.Views;

internal sealed class LoggingView : IView
{
    private readonly TextBox _logTextBox;

    public LoggingView()
    {
        _logTextBox = new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            Height = 150,
            Text = "Initial log entry..."
        };
    }

    public void AddTo(Controls root)
    {
        var clearButton = new Button
        {
            Content = "Clear Log",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 5, 0, 0)
        };
        clearButton.Click += (s, e) => _logTextBox.Text = string.Empty;

        root.Add(new TextBlock
        {
            Text = "Logging",
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        });
        root.Add(_logTextBox);
        root.Add(clearButton);
    }
}
