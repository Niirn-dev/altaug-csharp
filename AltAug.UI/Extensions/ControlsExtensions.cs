using AltAug.UI.Interfaces;
using Avalonia.Controls;

namespace AltAug.UI.Extensions;

internal static class ControlsExtensions
{
    public static void AddView(this Controls that, IView view) => view.AddTo(that);
}
