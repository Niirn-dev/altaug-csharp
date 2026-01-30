using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using Microsoft.Extensions.Logging;

namespace AltAug.Application.CraftingStrategies;

internal sealed partial class ChaosStrategy(IAutomationService automationService, ILogger<ChaosStrategy> logger) : ICraftingStrategy
{
    private readonly IAutomationService _automationService = automationService;
    private readonly ILogger<ChaosStrategy> _logger = logger;

    public async Task<int> ExecuteOperationAsync(
        IReadOnlyCollection<IFilter> conditions,
        ItemLocationParams locationParams,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        var item = new ItemInfo(_automationService.GetItemDescription(locationParams));
        LogInfoBeginningCraft(nameof(ChaosStrategy), item);
        if (item is not { Rarity: ItemRarity.Rare, IsCorrupted: false })
        {
            LogInfoItemNotValid(nameof(ChaosStrategy), item.Description);
            return 0;
        }

        if (conditions.All(c => c.IsMatch(item)))
        {
            LogInfoAlreadyCrafted();
            return 0;
        }

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _automationService.UseCurrency(CurrencyOrb.Chaos, locationParams);
            item = new ItemInfo(_automationService.GetItemDescription(locationParams));
            LogInfoCurrencyUsed(CurrencyOrb.Chaos, item);

            if (conditions.All(c => c.IsMatch(item)))
            {
                var currencyUsed = attempt + 1;
                LogInfoCraftingDone(nameof(ChaosStrategy), currencyUsed);
                return currencyUsed;
            }
        }

        LogInfoCraftingFailed(nameof(ChaosStrategy), maxAttempts);
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

    [LoggerMessage(Level = LogLevel.Information, Message = "Finished crafting the item with the {StrategyName}. Currency used: {CurrencyUsed}")]
    private partial void LogInfoCraftingDone(string strategyName, int currencyUsed);

    [LoggerMessage(Level = LogLevel.Information, Message = "Wasn't able to finish crafting the item with the {StrategyName}. Currency used: {CurrencyUsed}")]
    private partial void LogInfoCraftingFailed(string strategyName, int currencyUsed);
}
