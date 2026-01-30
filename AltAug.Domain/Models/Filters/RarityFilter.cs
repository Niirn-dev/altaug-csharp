using AltAug.Domain.Interfaces;

namespace AltAug.Domain.Models.Filters;

public sealed class RarityFilter(ItemRarity itemRarity) : IFilter
{
    private readonly ItemRarity _itemRarity = itemRarity;

    public bool IsMatch(ItemInfo item) => item.Rarity == _itemRarity;
}
