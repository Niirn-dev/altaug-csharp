namespace AltAug.UI.Interfaces;

internal interface IFilterControlFactory
{
    IFilterControl Create(Type filterType);
}
