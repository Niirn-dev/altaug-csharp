using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Enums;
using AltAug.Domain.Models.Filters;
using LanguageExt;

namespace AltAug.Domain.Models.CraftingStrategies;

internal sealed class AlterationStrategy(IAutomationService automationService) : ICraftingStrategy
{
    private readonly IAutomationService _automationService = automationService;

    public bool ExecuteOperation(IReadOnlyCollection<IFilter> conditions, ItemLocationParams locationParams, int maxAttempts)
    {
        var item = new ItemInfo(_automationService.GetItemDescription(locationParams));
        if (item.Rarity is not ItemRarity.Magic)
            return false;

        if (conditions.All(c => c.IsMatch(item)))
        {
            Console.WriteLine($"{nameof(AlterationStrategy)} finished crafting the item before even starting.");
            return true;
        }

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (conditions.Any(c => c is OpenPrefixFilter or OpenSuffixFilter)
                || item.IsAffixesFull())
            {
                _automationService.UseCurrency(CurrencyOrb.Alteration, locationParams);
            }
            else
            {
                _automationService.UseCurrency(CurrencyOrb.Augmentation, locationParams);
            }

            item = new ItemInfo(_automationService.GetItemDescription(locationParams));
            if (conditions.All(c => c.IsMatch(item)))
            {
                Console.WriteLine($"{nameof(AlterationStrategy)} finished crafting the item in {attempt + 1} attempts.");
                return true;
            }
        }

        Console.WriteLine($"{nameof(AlterationStrategy)} has reach max attempts before getting a successful craft.");
        return false;
    }
}
