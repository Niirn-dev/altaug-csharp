using AltAug.Domain.Interfaces;
using Avalonia.Controls;

namespace AltAug.UI.Interfaces;

internal interface IFilterControl
{
    bool IsRemoved { get; }
    Type FilterType { get; }
    IFilterParams Parameters { get; }

    void AddTo(Controls controls);
    void Accept(IFilterParams @params);
    IFilter MakeFilter();
}
