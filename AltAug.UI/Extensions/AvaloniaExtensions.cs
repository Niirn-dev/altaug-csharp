using AltAug.UI.Interfaces;
using Avalonia.Controls;

namespace AltAug.UI.Extensions;

internal static class AvaloniaExtensions
{
    public static void AddView(this Controls that, IView view) => view.AddTo(that);

    public static Grid AddControl(this Grid that, Control control, int row, int column)
    {
        control.SetValue(Grid.RowProperty, row);
        control.SetValue(Grid.ColumnProperty, column);

        that.Children.Add(control);

        return that;
    }
}
