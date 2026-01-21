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
            AutoGuiPause: 0.05,
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

public readonly record struct Point(double X, double Y)
{
    public static Point operator +(Point left, Point right) => new(
        X: left.X + right.X,
        Y: left.Y + right.Y);

    public static Point operator -(Point left, Point right) => new(
        X: left.X - right.X,
        Y: left.Y - right.Y);

    public static Point operator /(Point left, double right) => new(
        X: left.X / right,
        Y: left.Y / right);
}

public readonly record struct AutomationConfig(double AutoGuiPause, bool EnablePerfLogging);

public readonly record struct CraftingConfig();
