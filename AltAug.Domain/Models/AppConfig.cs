using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Enums;

namespace AltAug.Domain.Models;

public readonly record struct AppConfig(
    CoordinatesConfig CoordinatesConfig,
    AutomationConfig AutomationConfig,
    CraftingConfig CraftingConfig)
{
    public static AppConfig DefaultInstance { get; } = new(
        new(
            Item: new(0, 0),
            MapTopLeft: new(0, 0),
            MapBottomRight: new(0, 0),
            Alteration: new(0, 0),
            Augmentation: new(0, 0),
            Alchemy: new(0, 0),
            Scour: new(0, 0),
            Chaos: new(0, 0)
        ),
        new(
            AutoGuiPause: AutomationConfig.DefaultAutoGuiPause,
            CraftingStartDelay: AutomationConfig.DefaultCraftingStartDelay,
            ScreenResolution: AutomationConfig.DefaultScreenResolution
        ),
        new(
            ItemLocationIndex: 0,
            CraftingStrategyIndex: 0,
            ItemsToCraft: CraftingConfig.DefaultItemsToCraft,
            CurrencyToUseCount: CraftingConfig.DefaultCurrencyToUseCount,
            Filters: []
        )
    );
}

public readonly record struct CoordinatesConfig(
    Vec2 Item,
    Vec2 MapTopLeft,
    Vec2 MapBottomRight,
    Vec2 Alteration,
    Vec2 Augmentation,
    Vec2 Alchemy,
    Vec2 Scour,
    Vec2 Chaos);

public readonly record struct Vec2(double X, double Y)
{
    public static Vec2 operator +(Vec2 left, Vec2 right) => new(
        X: left.X + right.X,
        Y: left.Y + right.Y);

    public static Vec2 operator -(Vec2 left, Vec2 right) => new(
        X: left.X - right.X,
        Y: left.Y - right.Y);

    public static Vec2 operator /(Vec2 left, double right) => new(
        X: left.X / right,
        Y: left.Y / right);
}

public readonly record struct AutomationConfig(double AutoGuiPause, double CraftingStartDelay, Vec2 ScreenResolution)
{
    public const double DefaultAutoGuiPause = 0.05;
    public const double DefaultCraftingStartDelay = 1.5;
    public static readonly Vec2 DefaultScreenResolution = new(X: 1920, Y: 1080);
}

public readonly record struct CraftingConfig(
    int ItemLocationIndex,
    int CraftingStrategyIndex,
    int ItemsToCraft,
    int CurrencyToUseCount,
    (string FilterTypeName, IFilterParams Parameters)[] Filters)
{
    public const ItemLocation DefaultItemLocation = ItemLocation.CurrencyTab;
    public const int DefaultItemsToCraft = 1;
    public const int DefaultCurrencyToUseCount = 20;
}
