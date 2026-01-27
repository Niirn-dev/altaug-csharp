using Avalonia.Controls;
using LanguageExt;

namespace AltAug.UI.Extensions;

internal static class AvaloniaExtensions
{
    public static Grid AddControl(this Grid that, Control control, int row, int column)
    {
        control.SetValue(Grid.RowProperty, row);
        control.SetValue(Grid.ColumnProperty, column);

        that.Children.Add(control);

        return that;
    }

    public static Grid AddControl(this Grid that, Control control, int row, int column, Option<int> rowSpan, Option<int> columnSpan)
    {
        control.SetValue(Grid.RowProperty, row);
        control.SetValue(Grid.ColumnProperty, column);

        rowSpan.IfSome(r => control.SetValue(Grid.RowSpanProperty, r));
        columnSpan.IfSome(c => control.SetValue(Grid.ColumnSpanProperty, c));

        that.Children.Add(control);

        return that;
    }

    public static TValue SelectedValueOrDefault<TValue>(this ComboBox that, TValue defaultValue) => (TValue?)that.SelectedValue ?? defaultValue;
}
