using AltAug.Domain.Interfaces;
using Avalonia.Controls;

namespace AltAug.UI.Interfaces;

internal interface IFilterControl<TFilter> : IFilterControl
    where TFilter : IFilter
{
}

internal interface IFilterControl
{
    bool IsRemoved { get; }
    IFilterParams Parameters { get; }

    void Accept(IFilterParams @params);
    void AddTo(Controls controls);
    IFilter MakeFilter();
}
