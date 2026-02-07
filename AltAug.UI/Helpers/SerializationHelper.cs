using AltAug.Domain.Models.Filters;
using AltAug.UI.Interfaces;

namespace AltAug.UI.Helpers;

internal static class SerializationHelper
{
    public static string GetSerializedName(this IFilterControl that) => that switch
    {
        IFilterControl<AffixFilter> => nameof(AffixFilter),
        IFilterControl<OpenPrefixFilter> => nameof(OpenPrefixFilter),
        IFilterControl<OpenSuffixFilter> => nameof(OpenSuffixFilter),
        IFilterControl<RegexFilter> => nameof(RegexFilter),
        _ => throw new NotImplementedException(),
    };

    public static readonly IReadOnlyDictionary<string, Type> FilterSerializedNameToType = new Dictionary<string, Type>()
    {
        [ nameof(AffixFilter) ] = typeof(AffixFilter),
        [ nameof(OpenPrefixFilter) ] = typeof(OpenPrefixFilter),
        [ nameof(OpenSuffixFilter) ] = typeof(OpenSuffixFilter),
        [ nameof(RegexFilter) ] = typeof(RegexFilter),
    };
}
