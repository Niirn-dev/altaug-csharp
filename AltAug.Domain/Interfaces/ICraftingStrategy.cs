using AltAug.Domain.Models;

namespace AltAug.Domain.Interfaces;

public interface ICraftingStrategy
{
    bool ExecuteOperation(IReadOnlyCollection<IFilter> conditions, ItemLocationParams locationParams, int maxAttempts);
}
