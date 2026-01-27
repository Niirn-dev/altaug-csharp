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
        LogInfoBeginningCraft(nameof(AlterationStrategy), item);
        if (item is not { Rarity: ItemRarity.Magic, IsCorrupted: false })
            return 0;

        if (conditions.All(c => c.IsMatch(item)))
        {
            LogInfoAlreadyCrafted();
            return 0;
        }

        var currencyOrbUsed = CurrencyOrb.None;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var shouldUseAlt = conditions.Any(c => c is OpenPrefixFilter or OpenSuffixFilter)
                || item.IsAffixesFull();

            currencyOrbUsed = shouldUseAlt
                ? CurrencyOrb.Alteration
                : CurrencyOrb.Augmentation;

            _automationService.UseCurrency(currencyOrbUsed, locationParams);
            item = new ItemInfo(_automationService.GetItemDescription(locationParams));
            LogInfoCurrencyUsed(currencyOrbUsed, item);

            if (conditions.All(c => c.IsMatch(item)))
            {
                var currencyUsed = attempt + 1;
                LogInfoCraftingDone(currencyUsed, item);
                return currencyUsed;
            }
        }

        LogInfoCraftingFailed(maxAttempts, item);
        return maxAttempts;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting crafting with {StrategyName}. Initial item: {Item}")]
    private partial void LogInfoBeginningCraft(string strategyName, ItemInfo item);

    [LoggerMessage(Level = LogLevel.Information, Message = "Used {Currency}. Resulting item: {Item}")]
    private partial void LogInfoCurrencyUsed(CurrencyOrb currency, ItemInfo item);

    [LoggerMessage(Level = LogLevel.Information, Message = "The item already fulfills the crafting requirements.")]
    private partial void LogInfoAlreadyCrafted();

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished crafting the item. Currency used: {CurrencyUsed}. Final item: {Item}")]
    private partial void LogInfoCraftingDone(int currencyUsed, ItemInfo item);

    [LoggerMessage(Level = LogLevel.Information, Message = "Wasn't able to finish crafting the item. Currency used: {CurrencyUsed}. Final item: {Item}")]
    private partial void LogInfoCraftingFailed(int currencyUsed, ItemInfo item);
}
