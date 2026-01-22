using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using LanguageExt;

namespace AltAug.Domain.Interfaces;

public interface IAutomationService
{
    Option<Vec2> RecordMousePosition(int pollRate = 20, int failsafeTimeoutSeconds = 10);
    string GetItemDescription(ItemLocationParams locationParams);
    void UseCurrency(CurrencyOrb orb, ItemLocationParams locationParams);
}
