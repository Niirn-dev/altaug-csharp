using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Enums;
using AltAug.Domain.Models.Filters;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AltAug.Domain.Models.CraftingStrategies;

public sealed partial class AlterationStrategy(IAutomationService automationService, ILogger<AlterationStrategy> logger) : ICraftingStrategy
{
    private readonly IAutomationService _automationService = automationService;
    private readonly ILogger<AlterationStrategy> _logger = logger;

    public int ExecuteOperation(IReadOnlyCollection<IFilter> conditions, ItemLocationParams locationParams, int maxAttempts)
    {
        var item = new ItemInfo(_automationService.GetItemDescription(locationParams));
        if (item is not { Rarity: ItemRarity.Magic, IsCorrupted: false })
            return 0;

        if (conditions.All(c => c.IsMatch(item)))
        {
            LogInfoAlreadyCrafted();
            return 0;
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
                var currencyUsed = attempt + 1;
                LogInfoCraftingDone(currencyUsed);
                return currencyUsed;
            }
        }

        LogInfoCraftingFailed(maxAttempts);
        return maxAttempts;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "The item already fulfills the crafting requirements.")]
    private partial void LogInfoAlreadyCrafted();

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished crafting the item. Currency used: {CurrencyUsed}")]
    private partial void LogInfoCraftingDone(int currencyUsed);

    [LoggerMessage(Level = LogLevel.Information, Message = "Wasn't able to finish crafting the item. Currency used: {CurrencyUsed}")]
    private partial void LogInfoCraftingFailed(int currencyUsed);
}
