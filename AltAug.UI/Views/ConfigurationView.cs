using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AltAug.UI.Views;

internal sealed class ConfigurationView : IView
{
    public void AddTo(Controls root)
    {
        root.Add(new TextBlock
        {
            Text = "Configuration",
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        });
        root.Add(new TextBox());
    }
}
