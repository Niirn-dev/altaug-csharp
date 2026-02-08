using AltAug.Domain.Interfaces;
using LanguageExt;

namespace AltAug.Domain.Models.Filters;

public sealed class AffixFilter(Option<string> nameFilter, Option<string> descriptionFilter, Option<int> maxTierFilter, Option<AffixType> affixTypes) : IFilter
{
    private readonly Option<string> _nameFilter = nameFilter;
    private readonly Option<string> _descriptionFilter = descriptionFilter;
    private readonly Option<int> _maxTierFilter = maxTierFilter;
    private readonly Option<AffixType> _affixType = affixTypes;

    public bool IsMatch(ItemInfo item) => item.Affixes.Any(affix =>
        _nameFilter.ForAll(name => affix.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
        && _descriptionFilter.ForAll(desc => affix.Description.Contains(desc, StringComparison.OrdinalIgnoreCase))
        && _maxTierFilter.ForAll(maxTier => affix.TierValue <= maxTier)
        && _affixType.ForAll(type => affix.Type == type));
}
