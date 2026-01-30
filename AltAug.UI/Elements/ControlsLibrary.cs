using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AltAug.UI.Elements;

internal static class ControlsLibrary
{
    public static Button MakeSquareButton(object content) => new()
    {
        Height = 30,
        Width = 30,
        Content = content,
        FontWeight = FontWeight.Bold,
        Margin = new Thickness(uniformLength: 4),
        Padding = new Thickness(uniformLength: 0),
        HorizontalContentAlignment = HorizontalAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Center,
    };

    public static Button MakeFixedHeightButton(object content) => new()
    {
        Height = 30,
        Content = content,
        FontWeight = FontWeight.Bold,
        Margin = new Thickness(uniformLength: 4),
        Padding = new Thickness(horizontal: 4, vertical: 0),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        HorizontalContentAlignment = HorizontalAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Center,
    };

    public static TextBlock MakeTitleTextBlock(string text) => new()
    {
        Height = 30,
        Text = text,
        FontWeight = FontWeight.Bold,
        Margin = new Thickness(left: 0, top: 0, right: 0, bottom: 5),
        VerticalAlignment = VerticalAlignment.Center,
        TextAlignment = TextAlignment.Left,
    };

    public static TextBlock MakeVariableHeightTextBlock(string text) => new()
    {
        Text = text,
        Margin = new Thickness(uniformLength: 4),
        VerticalAlignment = VerticalAlignment.Center,
        TextAlignment = TextAlignment.Left,
        TextWrapping = TextWrapping.Wrap,
    };

    public static TextBlock MakeSingleLineTextBlock(string text) => new()
    {
        Height = 30,
        Text = text,
        Margin = new Thickness(uniformLength: 4),
        VerticalAlignment = VerticalAlignment.Center,
        TextAlignment = TextAlignment.Left,
    };

    public static TextBox MakeTextBox(string text) => new()
    {
        Height = 30,
        Text = text,
        Margin = new Thickness(uniformLength: 4),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Center,
        TextAlignment = TextAlignment.Left,
    };

    public static TextBox MakeLogTextBox() => new()
    {
        IsReadOnly = true,
        AcceptsReturn = true,
        TextWrapping = TextWrapping.Wrap,
        MinHeight = 150,
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Text = string.Empty,
    };

    public static ComboBox MakeComboBox() => new()
    {
        Height = 30,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Margin = new Thickness(uniformLength: 4),
    };

    public static NumericUpDown MakeIntUpDown(int value) => new()
    {
        Value = value,
        Height = 30,
        Width = 125,
        Margin = new Thickness(uniformLength: 4),
        Increment = 1,
        Minimum = 1,
        FormatString = "0",
    };

    public static Border MakeFilterBorder() => new()
    {
        BorderBrush = Brushes.DimGray,
        BorderThickness = new Thickness(uniformLength: 1),
        CornerRadius = new CornerRadius(uniformRadius: 5),
        Margin = new Thickness(uniformLength: 2),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        MaxWidth = 600,
    };

    public static CheckBox MakeCheckBox(object content) => new()
    {
        Content = content,
        Height = 30,
        Margin = new Thickness(uniformLength: 4),
    };
}
