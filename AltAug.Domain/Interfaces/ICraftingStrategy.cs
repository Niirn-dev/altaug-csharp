namespace AltAug.Domain.Interfaces;

public interface ICraftingStrategy
{
    bool ExecuteOperation(IReadOnlyCollection<IFilter> conditions, int maxAttempts);
}
