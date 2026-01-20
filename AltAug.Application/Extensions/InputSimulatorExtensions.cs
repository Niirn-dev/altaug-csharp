using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using LanguageExt;
using WindowsInput;

namespace AltAug.Application.Extensions;

internal static class InputSimulatorExtensions
{
    private const int InventoryRowCount = 5;

    public static IMouseSimulator HoverItem(this IMouseSimulator that, AppConfig appConfig, ItemLocation location, Option<int> inventoryPosition)
    {
        return location switch
        {
            ItemLocation.CurrencyTab => that.HoverCurrencyTabItem(appConfig),
            ItemLocation.Inventory => inventoryPosition
                .Match(
                    s => that.HoverInventoryItem(appConfig, s),
                    () => throw new ArgumentException($"Expected {nameof(inventoryPosition)} to be some when {nameof(location)} is {ItemLocation.Inventory}.")
                ),
            _ => throw new ArgumentException($"Unsupported {nameof(location)} enum value."),
        };
    }

    public static IMouseSimulator HoverCurrency(this IMouseSimulator that, AppConfig appConfig, CurrencyOrb orb)
    {
        var coordinatesConfig = appConfig.CoordinatesConfig;

        return orb switch
        {
            CurrencyOrb.Alteration => that.MoveMouseTo(coordinatesConfig.Alteration),
            CurrencyOrb.Augmentation => that.MoveMouseTo(coordinatesConfig.Augmentation),
            CurrencyOrb.Alchemy => that.MoveMouseTo(coordinatesConfig.Alchemy),
            CurrencyOrb.Scouring => that.MoveMouseTo(coordinatesConfig.Scour),
            CurrencyOrb.Chaos => that.MoveMouseTo(coordinatesConfig.Chaos),
            _ => throw new ArgumentException($"Unsupported {nameof(orb)} enum value."),
        };
    }

    public static IMouseSimulator HoverCurrencyTabItem(this IMouseSimulator that, AppConfig appConfig) => that
        .MoveMouseTo(appConfig.CoordinatesConfig.Item);

    public static IMouseSimulator HoverInventoryItem(this IMouseSimulator that, AppConfig appConfig, int inventoryPosition)
    {
        static Point CalculateInventoryPosition(AppConfig appConfig, int inventoryPosition)
        {
            var coordinates = appConfig.CoordinatesConfig;
            var mapPosition = (coordinates.MapTopLeft + coordinates.MapBottomRight) / 2;
            var step = coordinates.MapBottomRight - coordinates.MapTopLeft;

            var row = inventoryPosition % InventoryRowCount;
            var col = inventoryPosition / InventoryRowCount;
            Point delta = new(X: step.X * col, Y: step.Y * row);

            return mapPosition + delta;
        }

        return that
            .MoveMouseTo(CalculateInventoryPosition(appConfig, inventoryPosition));
    }

    public static IMouseSimulator MoveMouseTo(this IMouseSimulator that, Point point) => that
        .MoveMouseTo(point.X, point.Y);
}
