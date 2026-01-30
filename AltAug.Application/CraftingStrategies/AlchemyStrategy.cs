using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using Microsoft.Extensions.Logging;

namespace AltAug.Application.CraftingStrategies;

internal sealed partial class AlchemyStrategy(IAutomationService automationService, ILogger<AlchemyStrategy> logger) : ICraftingStrategy
{
    private readonly IAutomationService _automationService = automationService;
    private readonly ILogger<AlchemyStrategy> _logger = logger;

    public async Task<int> ExecuteOperationAsync(
        IReadOnlyCollection<IFilter> conditions,
        ItemLocationParams locationParams,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        var item = new ItemInfo(_automationService.GetItemDescription(locationParams));
        LogInfoBeginningCraft(nameof(AlchemyStrategy), item);
        if (item is not { Rarity: ItemRarity.Normal or ItemRarity.Magic or ItemRarity.Rare, IsCorrupted: false })
        {
            LogInfoItemNotValid(nameof(AlchemyStrategy), item.Description);
            return 0;
        }

        if (conditions.All(c => c.IsMatch(item)))
        {
            LogInfoAlreadyCrafted();
            return 0;
        }

        var currencyOrbUsed = CurrencyOrb.None;
        var (alchemyUsedCount, scourUsedCount) = (0, 0);
        while (Math.Max(alchemyUsedCount, scourUsedCount) < maxAttempts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (item.Rarity is ItemRarity.Normal)
            {
                currencyOrbUsed = CurrencyOrb.Alchemy;
                alchemyUsedCount++;
            }
            else
            {
                currencyOrbUsed = CurrencyOrb.Scouring;
                scourUsedCount++;
            }

            _automationService.UseCurrency(currencyOrbUsed, locationParams);
            item = new ItemInfo(_automationService.GetItemDescription(locationParams));
            LogInfoCurrencyUsed(currencyOrbUsed, item);

            if (conditions.All(c => c.IsMatch(item)))
            {
                LogInfoCraftingDone(nameof(AlchemyStrategy), alchemyUsedCount, scourUsedCount);
                return alchemyUsedCount + scourUsedCount;
            }
        }

        LogInfoCraftingFailed(nameof(AlchemyStrategy), alchemyUsedCount, scourUsedCount);
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

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished crafting the item with the {StrategyName}. Alchemies used: {AlchemyUsedCount}. Scours used: {ScourUsedCount}")]
    private partial void LogInfoCraftingDone(string strategyName, int alchemyUsedCount, int scourUsedCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Wasn't able to finish crafting the item with the {StrategyName}. Alchemies used: {AlchemyUsedCount}. Scours used: {ScourUsedCount}")]
    private partial void LogInfoCraftingFailed(string strategyName, int alchemyUsedCount, int scourUsedCount);
}
