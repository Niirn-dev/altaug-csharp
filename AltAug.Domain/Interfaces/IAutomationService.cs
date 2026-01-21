using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using LanguageExt;

namespace AltAug.Domain.Interfaces;

public interface IAutomationService
{
    Option<Point> RecordMousePosition(int pollRate = 20, int failsafeTimeoutSeconds = 10);
    string GetItemDescription(ItemLocation location, Option<int> inventoryPosition);
    void UseCurrency(CurrencyOrb orb, ItemLocation location, Option<int> inventoryPosition);
}
