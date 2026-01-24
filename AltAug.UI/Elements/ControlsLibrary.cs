using AltAug.Domain.Models.Filters;
using AltAug.UI.Interfaces;

namespace AltAug.UI.Elements;

internal static class ControlsLibrary
{
    public static class FilterControlFactory
    {
        public static IFilterControl MakeFilterControl(Type filterType) => filterType switch
        {
            Type t when t == typeof(OpenPrefixFilter) => new OpenPrefixFilterControl(),
            Type t when t == typeof(OpenSuffixFilter) => new OpenSuffixFilterControl(),
            Type t when t == typeof(RegexFilter) => new RegexFilterControl(),
            _ => throw new NotImplementedException(),
        };
    }
}
