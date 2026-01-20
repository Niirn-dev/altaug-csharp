using AltAug.Domain.Models.Enums;
using LanguageExt;

namespace AltAug.Domain.Interfaces;

internal interface ICraftingService
{
    bool Craft(ICraftingStrategy strategy, IReadOnlyCollection<IFilter> conditions, ItemLocation location, Option<int> inventoryPosition, int maxAttempts);
}
