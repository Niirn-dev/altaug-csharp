using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;

namespace AltAug.Application.Services;

internal sealed class CraftingService : ICraftingService
{
    public async Task CraftItemsAsync(
        ICraftingStrategy strategy,
        IReadOnlyCollection<IFilter> conditions,
        ItemLocationParams locationParams,
        int itemsCount,
        int maxAttempts,
        CancellationToken cancellationToken)
    {
        int currencyUsed = 0;

        if (conditions.Count == 0)
            return;

        for (var i = 0; i < itemsCount && currencyUsed < maxAttempts; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currencyUsed += await strategy.ExecuteOperationAsync(conditions, locationParams, maxAttempts - currencyUsed, cancellationToken);

            locationParams = locationParams with { InventoryPosition = locationParams.InventoryPosition.Map(s => s++) };
        }
    }
}
