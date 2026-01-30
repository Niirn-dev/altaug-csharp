using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using AltAug.Domain.Models.Filters;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AltAug.Application.CraftingStrategies;

internal sealed partial class AlterationStrategy(IAutomationService automationService, ILogger<AlterationStrategy> logger) : ICraftingStrategy
{
    private readonly IAutomationService _automationService = automationService;
    private readonly ILogger<AlterationStrategy> _logger = logger;

    public async Task<int> ExecuteOperationAsync(
        IReadOnlyCollection<IFilter> conditions,
        ItemLocationParams locationParams,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        var item = new ItemInfo(_automationService.GetItemDescription(locationParams));
        LogInfoBeginningCraft(nameof(AlterationStrategy), item);
        if (item is not { Rarity: ItemRarity.Magic, IsCorrupted: false })
        {
            LogInfoItemNotValid(nameof(AlterationStrategy), item.Description);
            return 0;
        }

        if (conditions.All(c => c.IsMatch(item)))
        {
            LogInfoAlreadyCrafted();
            return 0;
        }

        var currencyOrbUsed = CurrencyOrb.None;
        var (altUsedCount, augUsedCount) = (0, 0);
        while (Math.Max(altUsedCount, augUsedCount) < maxAttempts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (conditions.Any(c => c is OpenPrefixFilter or OpenSuffixFilter)
                || item.IsAffixesFull())
            {
                currencyOrbUsed = CurrencyOrb.Alteration;
                altUsedCount++;
            }
            else
            {
                currencyOrbUsed = CurrencyOrb.Augmentation;
                augUsedCount++;
            }

            _automationService.UseCurrency(currencyOrbUsed, locationParams);
            item = new ItemInfo(_automationService.GetItemDescription(locationParams));
            LogInfoCurrencyUsed(currencyOrbUsed, item);

            if (conditions.All(c => c.IsMatch(item)))
            {
                LogInfoCraftingDone(nameof(AlterationStrategy), altUsedCount, augUsedCount);
                return altUsedCount + augUsedCount;
            }
        }

        LogInfoCraftingFailed(nameof(AlterationStrategy), altUsedCount, augUsedCount);
        return maxAttempts;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting crafting with {StrategyName}. Initial item: {Item}")]
    private partial void LogInfoBeginningCraft(string strategyName, ItemInfo item);

    [LoggerMessage(Level = LogLevel.Information, Message = "The item is not valid for crafting with {StrategyName}. Full item description: {ItemDescription}")]
    private partial void LogInfoItemNotValid(string strategyName, string itemDescription);

    [LoggerMessage(Level = LogLevel.Information, Message = "Used {Currency}. Resulting item: {Item}")]
    private partial void LogInfoCurrencyUsed(CurrencyOrb currency, ItemInfo item);

    [LoggerMessage(Level = LogLevel.Information, Message = "The item already fulfills the crafting requirements.")]
    private partial void LogInfoAlreadyCrafted();

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished crafting the item with the {StrategyName}. Alterations used: {AltUsedCount}. Augmentations used: {AugUsedCount}")]
    private partial void LogInfoCraftingDone(string strategyName, int altUsedCount, int augUsedCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Wasn't able to finish crafting the item with the {StrategyName}. Alterations used: {AltUsedCount}. Augmentations used: {AugUsedCount}")]
    private partial void LogInfoCraftingFailed(string strategyName, int altUsedCount, int augUsedCount);
}
