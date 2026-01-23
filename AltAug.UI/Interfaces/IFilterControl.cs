using AltAug.Domain.Interfaces;
using Avalonia.Controls;

namespace AltAug.UI.Interfaces;

internal interface IFilterControl
{
    bool IsRemoved { get; }
    void AddToContainer(StackPanel container);
    IFilter MakeFilter();
}
