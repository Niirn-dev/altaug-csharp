using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Filters;
using AltAug.UI.Interfaces;

namespace AltAug.UI.Elements;

internal sealed class FilterControlFactory(IStateManager<RegexLibraryStore> regexLibraryManager) : IFilterControlFactory
{
    private readonly IStateManager<RegexLibraryStore> _regexLibraryManager = regexLibraryManager;

    public IFilterControl Create(Type filterType) => filterType switch
    {
        Type t when t == typeof(OpenPrefixFilter) => new OpenPrefixFilterControl(),
        Type t when t == typeof(OpenSuffixFilter) => new OpenSuffixFilterControl(),
        Type t when t == typeof(RegexFilter) => new RegexFilterControl(_regexLibraryManager),
        _ => throw new NotImplementedException(),
    };
}
