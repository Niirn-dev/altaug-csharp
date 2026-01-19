using AltAug.UI.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AltAug.UI.Views;

internal sealed class CraftingView : IView
{
    public void AddTo(Controls root)
    {
        root.Add(new TextBlock
        {
            Text = "Crafting Target",
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        });
        root.Add(new TextBox());
    }
}
