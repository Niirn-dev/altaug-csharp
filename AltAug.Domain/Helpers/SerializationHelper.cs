using AltAug.Domain.Models.Filters;

namespace AltAug.Domain.Helpers;

public static class SerializationHelper
{
    public static readonly IReadOnlyDictionary<string, string> TypeNameToSerializedName = new Dictionary<string, string>()
    {
        [ typeof(OpenPrefixFilter).Name] = nameof(OpenPrefixFilter),
        [ typeof(OpenSuffixFilter).Name] = nameof(OpenSuffixFilter),
        [ typeof(RegexFilter).Name] = nameof(RegexFilter),
    };

    public static readonly IReadOnlyDictionary<string, Type> SerializedNameToType = new Dictionary<string, Type>()
    {
        [ nameof(OpenPrefixFilter) ] = typeof(OpenPrefixFilter),
        [ nameof(OpenSuffixFilter) ] = typeof(OpenSuffixFilter),
        [ nameof(RegexFilter) ] = typeof(RegexFilter),
    };
}
