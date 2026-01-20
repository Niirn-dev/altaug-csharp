using System.Text.RegularExpressions;
using AltAug.Domain.Interfaces;

namespace AltAug.Domain.Models.Filters;

public sealed class RegexFilter : IFilter
{
    private readonly bool _isInverted;
    private readonly Regex _regex;

    public RegexFilter(string regexString)
    {
        _isInverted = regexString.StartsWith('!');
        _regex = _isInverted 
            ? new(regexString[1..], RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase)
            : new(regexString, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }

    public bool IsMatch(ItemInfo item) => item.Rarity switch
    {
        ItemRarity.Normal => false,
        ItemRarity.Magic or ItemRarity.Rare => _regex.IsMatch(item.Description) ^ _isInverted,
        _ => throw new ArgumentException("Crafting is only expected on Normal, Magic or Rare items."),
    };
}
