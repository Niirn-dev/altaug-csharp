using AltAug.Domain.Interfaces;

namespace AltAug.UI.Interfaces;

internal interface IFilterControlFactory
{
    IFilterControl Create(Type filterType);
}
