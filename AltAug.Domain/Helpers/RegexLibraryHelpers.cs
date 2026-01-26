using AltAug.Domain.Models;

namespace AltAug.Domain.Helpers;

public static class RegexLibraryHelpers
{
    public static IReadOnlyCollection<string> GetItemTypes(this RegexLibraryStore that) => that.TypeToBaseToNameToValue.Keys;

    public static IReadOnlyCollection<string> GetItemBases(
        this RegexLibraryStore that,
        string itemType)
    {
        if (!that.TypeToBaseToNameToValue.TryGetValue(itemType, out var basesDict))
            return [];

        return basesDict.Keys;
    }

    public static IReadOnlyCollection<string> GetRegexTitles(
        this RegexLibraryStore that,
        string itemType,
        string itemBase)
    {
        if (!that.TypeToBaseToNameToValue.TryGetValue(itemType, out var basesDict))
            return [];

        if (!basesDict.TryGetValue(itemBase, out var regexTitlesDict))
            return [];

        return regexTitlesDict.Keys;
    }


    public static string GetRegexString(
        this RegexLibraryStore that,
        string itemType,
        string itemBase,
        string regexTitle)
    {
        if (!that.TypeToBaseToNameToValue.TryGetValue(itemType, out var basesDict))
            return string.Empty;

        if (!basesDict.TryGetValue(itemBase, out var regexTitlesDict))
            return string.Empty;

        if (!regexTitlesDict.TryGetValue(regexTitle, out var regexString))
            return string.Empty;

        return regexString;
    }
}
