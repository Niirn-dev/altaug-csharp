using AltAug.UI.Elements;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using AltAug.UI.Logging;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Views;

public sealed class LoggingView(ILoggerFactory loggerFactory) : IView
{
    private const string ViewTitle = "Logging";

    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    public Control GetControl()
    {
        var grid = new Grid
        {
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, *"),
        };
        var clearButton = ControlsLibrary.MakeFixedHeightButton(content: "Clear Log");

        var logTextBox = new TextBox()
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 150,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Text = string.Empty,
        };
        clearButton.Click += (_, _) => logTextBox.Text = string.Empty;

        grid.AddControl(ControlsLibrary.MakeTitleTextBlock(text: ViewTitle), row: 0, column: 0);
        grid.AddControl(clearButton, row: 1, column: 0);
        grid.AddControl(logTextBox, row: 2, column: 0);

        _loggerFactory.AddProvider(new TextBoxLoggerProvider(logTextBox));

        return grid;
    }
}
