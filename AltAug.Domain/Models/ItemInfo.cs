using System.Diagnostics;
using System.Text;
using AltAug.Domain.Extensions;
using AltAug.Domain.Helpers;
using LanguageExt;
using MoreLinq;

namespace AltAug.Domain.Models;

public sealed class ItemInfo
{
    public string Description { get; init; }
    public ItemType ItemType { get; init; } = ItemType.Gear;
    public ItemRarity Rarity { get; init; } = ItemRarity.None;
    public List<AffixInfo> Affixes { get; init; } = [];
    public bool IsCorrupted { get; init; } = false;

    public IReadOnlyList<AffixInfo> Prefixes { get => [.. Affixes.Where(a => a.Type is AffixType.Prefix or AffixType.Any)]; }
    public IReadOnlyList<AffixInfo> Suffixes { get => [.. Affixes.Where(a => a.Type is AffixType.Suffix or AffixType.Any)]; }

    public ItemInfo(string advancedDescription)
    {
        Description = advancedDescription.Replace("\r\n", "\n").Replace("\r", "\n");

        var itemBaseMatch = RegexHelper.ItemBaseRegex.Match(Description);
        if (itemBaseMatch.Success)
        {
            ItemType = itemBaseMatch.Groups[RegexHelper.ItemBaseRegexGroupClass] switch
            {
                { Success: true } grp => grp.Value.ParseEnum<ItemType>(ignoreCase: true).IfNone(() => ItemType.Gear),
                _ => ItemType.Gear,
            };
            Rarity = itemBaseMatch.Groups[RegexHelper.ItemBaseRegexGroupRarity] switch
            {
                { Success: true } grp => grp.Value.ParseEnum<ItemRarity>(ignoreCase: true).IfNone(() => ItemRarity.None),
                _ => ItemRarity.None,
            };
            IsCorrupted = itemBaseMatch.Groups[RegexHelper.ItemBaseRegexGroupCorrupted] switch
            {
                { Success: true } => true,
                _ => false,
            };
        }

        var affixMatches = RegexHelper.AffixRegex.Matches(Description);
        affixMatches.ForEach(m =>
        {
            Debug.Assert(m.Success, "Expected the Regex match to always be true here.");

            AffixInfo affix = new(
                Type: m.Groups[RegexHelper.AffixRegexGroupAffixType].Value
                    .ParseEnum<AffixType>(ignoreCase: true)
                    .IfNone(AffixType.Any),
                Name: m.Groups[RegexHelper.AffixRegexGroupAffixName].Value,
                TierTitle: m.Groups[RegexHelper.AffixRegexGroupTierTitle].Value,
                TierValue: int.Parse(m.Groups[RegexHelper.AffixRegexGroupTierValue].Value),
                Description: m.Groups[RegexHelper.AffixRegexGroupDescription].Value
            );

            Affixes.Add(affix);
        });
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Item affixes:");
        Affixes.ForEach(affix => sb.AppendLine($"- {affix}"));

        sb.Length--;

        return sb.ToString();
    }

    public bool IsAffixesFull() => Rarity switch
    {
        ItemRarity.Magic => (Prefixes.Count + Suffixes.Count) >= 2,
        ItemRarity.Rare => (Prefixes.Count + Suffixes.Count) >= 6,
        _ => true,
    };
}

public sealed record AffixInfo(AffixType Type, string Name, string TierTitle, int TierValue, string Description)
{
    public string Tier { get => $"{TierTitle}: {TierValue}"; }

    public override string ToString() => $"{Type} | {Name} | {Tier} | {Description}";
}

public enum ItemRarity
{
    None = 0,
    Normal = 1,
    Magic = 2,
    Rare = 3,
    Unique = 4,
}

public enum ItemType
{
    Gear = 1,
    Maps = 2,
}

public enum AffixType
{
    Any = 0,
    Prefix = 1,
    Suffix = 2,
}
