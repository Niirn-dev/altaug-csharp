using System.Text.RegularExpressions;

namespace AltAug.Domain.Helpers;

public static partial class RegexHelper
{
    public const string ItemBaseRegexGroupClass = "class";
    public const string ItemBaseRegexGroupRarity = "rarity";
    public const string ItemBaseRegexGroupCorrupted = "corrupted";

    [GeneratedRegex(
        @"^Item Class: (?<class>[^\r\n]*)$\s*^Rarity:\s(?<rarity>[^\r\n]*)$(?<corrupted>.*^Corrupted$)?",
        RegexOptions.Multiline | RegexOptions.Singleline)]
    public static partial Regex ItemBaseRegex { get; }

    public const string AffixRegexGroupAffixType = "affix_type";
    public const string AffixRegexGroupAffixName = "affix_name";
    public const string AffixRegexGroupTierTitle = "tier_title";
    public const string AffixRegexGroupTierValue = "tier_value";
    public const string AffixRegexGroupDescription = "description";

    [GeneratedRegex(
        @"^{ (?:Master Crafted )?(?<affix_type>Prefix|Suffix) Modifier \""(?<affix_name>[\w\s'-]*)\"" (?:\((?<tier_title>Rank|Tier): (?<tier_value>\d*)\))?[^(?:\r\n|\n|\r)]*$(?:\r\n|\n|\r)(?<description>[^{]*?)(?=(?:\n\n))",
        RegexOptions.Multiline)]
    public static partial Regex AffixRegex { get; }
}
