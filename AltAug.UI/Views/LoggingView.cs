using AltAug.UI.Elements;
using AltAug.UI.Extensions;
using AltAug.UI.Interfaces;
using AltAug.UI.Logging;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Views;

public sealed class LoggingView(ILoggerFactory loggerFactory) : IView
{
    private const string ViewTitle = "Logging";

    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    public Control GetControl()
    {
        // Initialize controls
        var grid = new Grid
        {
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, *"),
        };
        var clearButton = ControlsLibrary.MakeFixedHeightButton(content: "Clear Log");

        var logTextBox = ControlsLibrary.MakeLogTextBox();
        logTextBox.MaxHeight = 450;
        clearButton.Click += (_, _) => logTextBox.Text = string.Empty;

        // Define layout
        grid.AddControl(ControlsLibrary.MakeTitleTextBlock(text: ViewTitle), row: 0, column: 0)
            .AddControl(clearButton, row: 1, column: 0)
            .AddControl(logTextBox, row: 2, column: 0);

        _loggerFactory.AddProvider(new TextBoxLoggerProvider(logTextBox, useMinimalFormat: false));

        return grid;
    }
}
