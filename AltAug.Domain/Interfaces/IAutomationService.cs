using AltAug.Domain.Models.Enums;
using LanguageExt;

namespace AltAug.Domain.Interfaces;

public interface IAutomationService
{
    string GetItemDescription(ItemLocation location, Option<int> inventoryPosition);

    void UseCurrency(CurrencyOrb orb, ItemLocation location, Option<int> inventoryPosition);
}
