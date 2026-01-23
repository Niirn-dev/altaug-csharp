using AltAug.Domain.Models;

namespace AltAug.Domain.Interfaces;

public interface ICraftingService
{
    void CraftItems(
        ICraftingStrategy strategy,
        IReadOnlyCollection<IFilter> conditions,
        ItemLocationParams locationParams,
        int itemsCount,
        int maxAttempts);
}
