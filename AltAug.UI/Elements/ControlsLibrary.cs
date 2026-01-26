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
        Padding = new Thickness(uniformLength: 0),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        HorizontalContentAlignment = HorizontalAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Center,
    };

    public static TextBlock MakeTitleTextBlock(string content) => new()
    {
        Height = 30,
        Text = content,
        FontWeight = FontWeight.Bold,
        Margin = new Thickness(left: 0, top: 0, right: 0, bottom: 5),
    };

    public static TextBox MakeTextBox(string text) => new()
    {
        Height = 30,
        Text = text,
        HorizontalAlignment = HorizontalAlignment.Stretch,
    };

    public static ComboBox MakeComboBox() => new()
    {
        Height = 30,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Margin = new Thickness(uniformLength: 4),
    };
}
