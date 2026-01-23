using AltAug.Domain.Models.Enums;
using LanguageExt;

namespace AltAug.Domain.Models;

public sealed record ItemLocationParams(
    ItemLocation Location,
    Option<int> InventoryPosition,
    Option<Vec2> ItemDimensions);
