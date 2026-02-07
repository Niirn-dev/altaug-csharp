using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Enums;

namespace AltAug.Domain.Models;

public readonly record struct AppConfig(
    MainWindowConfig MainWindowConfig,
    CoordinatesConfig CoordinatesConfig,
    AutomationConfig AutomationConfig,
    CraftingConfig CraftingConfig)
{
    public static AppConfig DefaultInstance { get; } = new(
        MainWindowConfig: new(
            Height: MainWindowConfig.DefaultHeight,
            Width: MainWindowConfig.DefaultWidth    
        ),
        CoordinatesConfig: new(
            Item: new(0, 0),
            InventorySlotTopLeft: new(0, 0),
            InventorySlotBottomRight: new(0, 0),
            Alteration: new(0, 0),
            Augmentation: new(0, 0),
            Alchemy: new(0, 0),
            Scour: new(0, 0),
            Chaos: new(0, 0)
        ),
        AutomationConfig: new(
            AutoGuiPause: AutomationConfig.DefaultAutoGuiPause,
            CraftingStartDelay: AutomationConfig.DefaultCraftingStartDelay,
            ScreenResolution: AutomationConfig.DefaultScreenResolution
        ),
        CraftingConfig: new(
            ItemLocationIndex: 0,
            CraftingStrategyIndex: 0,
            ItemsToCraft: CraftingConfig.DefaultItemsToCraft,
            CurrencyToUseCount: CraftingConfig.DefaultCurrencyToUseCount,
            Filters: []
        )
    );
}

public readonly record struct MainWindowConfig(double Height, double Width)
{
    public const double DefaultHeight = 800;
    public const double DefaultWidth = 800;
}

public readonly record struct CoordinatesConfig(
    Vec2 Item,
    Vec2 InventorySlotTopLeft,
    Vec2 InventorySlotBottomRight,
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

    public static Vec2 operator *(Vec2 left, Vec2 right) => new(
        X: left.X * right.X,
        Y: left.Y * right.Y);
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
