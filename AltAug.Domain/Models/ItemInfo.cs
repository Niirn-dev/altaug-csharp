using System.Diagnostics;
using AltAug.Domain.Helpers;
using MoreLinq;

namespace AltAug.Domain.Models;

public sealed class ItemInfo
{
    public string Description { get; init; }
    public ItemType ItemType { get; init; } = ItemType.Gear;
    public ItemRarity Rarity { get; init; } = ItemRarity.None;
    public List<AffixInfo> Prefixes { get; init; } = [];
    public List<AffixInfo> Suffixes { get; init; } = [];
    public bool IsCorrupted { get; init; } = false;

    public ItemInfo(string advancedDescription)
    {
        Description = advancedDescription.Replace("\r\n", "\n").Replace("\r", "\n");

        var itemBaseMatch = RegexHelper.ItemBaseRegex.Match(Description);
        if (itemBaseMatch.Success)
        {
            ItemType = itemBaseMatch.Groups[RegexHelper.ItemBaseRegexGroupClass] switch
            {
                { Success: true } grp => OptionHelpers.ParseEnum<ItemType>(grp.Value, ignoreCase: true).IfNone(() => ItemType.Gear),
                _ => ItemType.Gear,
            };
            Rarity = itemBaseMatch.Groups[RegexHelper.ItemBaseRegexGroupRarity] switch
            {
                { Success: true } grp => OptionHelpers.ParseEnum<ItemRarity>(grp.Value, ignoreCase: true).IfNone(() => ItemRarity.None),
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

            AffixInfo info = new(
                Type: m.Groups[RegexHelper.AffixRegexGroupAffixType].Value,
                Name: m.Groups[RegexHelper.AffixRegexGroupAffixName].Value,
                Tier: m.Groups[RegexHelper.AffixRegexGroupTier].Value,
                Description: m.Groups[RegexHelper.AffixRegexGroupDescription].Value
            );

            if (info.Type.Equals("prefix", StringComparison.OrdinalIgnoreCase))
            {
                Prefixes.Add(info);
            }
            else
            {
                Suffixes.Add(info);
            }
        });
    }

    public bool IsAffixesFull() => Rarity switch
    {
        ItemRarity.Magic => (Prefixes.Count + Suffixes.Count) >= 2,
        ItemRarity.Rare => (Prefixes.Count + Suffixes.Count) >= 6,
        _ => true,
    };
}

public sealed record AffixInfo(string Type, string Name, string Tier, string Description);

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
    Map = 2,
}
