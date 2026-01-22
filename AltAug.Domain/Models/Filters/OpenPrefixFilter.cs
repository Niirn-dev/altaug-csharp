using AltAug.Domain.Interfaces;

namespace AltAug.Domain.Models.Filters;

public sealed class OpenPrefixFilter : IFilter
{
    public bool IsMatch(ItemInfo item) => item.Rarity switch
    {
        ItemRarity.Magic => item.Prefixes.Count < 1,
        ItemRarity.Rare => item.Prefixes.Count < 3,
        _ => false,
    };
}
