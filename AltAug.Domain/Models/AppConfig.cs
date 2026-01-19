namespace AltAug.Domain.Models;

public record struct AppConfig(
    CoordinatesConfig CoordinatesConfig,
    AutomationConfig AutomationConfig,
    CraftingConfig CraftingConfig)
{
    public static AppConfig DefaultInstance { get; } = new(
        new(
            Item: new(0m, 0m),
            MapTopLeft: new(0m, 0m),
            MapBottomRight: new(0m, 0m),
            Alteration: new(0m, 0m),
            Augmentation: new(0m, 0m),
            Alchemy: new(0m, 0m),
            Scour: new(0m, 0m),
            Chaos: new(0m, 0m)
        ),
        new(
            AutoGuiPause: 0.05m,
            EnablePerfLogging: false
        ),
        new()
    );
}

public record struct CoordinatesConfig(
    Point Item,
    Point MapTopLeft,
    Point MapBottomRight,
    Point Alteration,
    Point Augmentation,
    Point Alchemy,
    Point Scour,
    Point Chaos);

public record struct Point(decimal X, decimal Y);

public record struct AutomationConfig(decimal AutoGuiPause, bool EnablePerfLogging);

public record struct CraftingConfig();
