using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;

namespace AltAug.Application.Services;

internal sealed class CraftingService : ICraftingService
{
    public void CraftItems(ICraftingStrategy strategy, IReadOnlyCollection<IFilter> conditions, ItemLocationParams locationParams, int itemsCount, int maxAttempts)
    {
        int currencyUsed = 0;

        for (var i = 0; i < itemsCount && currencyUsed < maxAttempts; i++)
        {
            currencyUsed += strategy.ExecuteOperation(conditions, locationParams, maxAttempts - currencyUsed);

            locationParams = locationParams with { InventoryPosition = locationParams.InventoryPosition.Map(s => s++) };
        }
    }
}
