using AltAug.Domain.Models;

namespace AltAug.Domain.Interfaces;

public interface ICraftingStrategy
{
    Task<int> ExecuteOperationAsync(IReadOnlyCollection<IFilter> conditions, ItemLocationParams locationParams, int maxAttempts, CancellationToken cancellationToken);
}
