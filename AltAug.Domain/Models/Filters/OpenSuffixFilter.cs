using AltAug.Domain.Interfaces;

namespace AltAug.Domain.Models.Filters;

public sealed class OpenSuffixFilter : IFilter
{
    public bool IsMatch(ItemInfo item) => item.Rarity switch
    {
        ItemRarity.Magic => item.Suffixes.Count < 1,
        ItemRarity.Rare => item.Suffixes.Count < 3,
        _ => false,
    };
}
