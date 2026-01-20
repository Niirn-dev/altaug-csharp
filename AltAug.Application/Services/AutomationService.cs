using AltAug.Application.Extensions;
using AltAug.Application.Wrappers;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Enums;
using LanguageExt;
using TextCopy;
using WindowsInput;

namespace AltAug.Application.Services;

internal sealed class AutomationService(IStateManager stateManager) : IAutomationService
{
    private readonly IStateManager _stateManager = stateManager;

    public string GetItemDescription(ItemLocation location, Option<int> inventoryPosition)
    {
        GetDelayedInputSimulator()
            .Mouse
            .HoverItem(_stateManager.AppConfig, location, inventoryPosition)
            .Keyboard
            .ModifiedKeyStroke([VirtualKeyCode.CONTROL, VirtualKeyCode.LMENU], VirtualKeyCode.VK_C);

        return ClipboardService.GetText() ?? string.Empty;
    }

    public void UseCurrency(CurrencyOrb orb, ItemLocation location, Option<int> inventoryPosition) => GetDelayedInputSimulator()
        .Mouse
        .HoverCurrency(_stateManager.AppConfig, orb)
        .RightButtonClick()
        .HoverItem(_stateManager.AppConfig, location, inventoryPosition)
        .LeftButtonClick();

    private DelayedInputSimulator GetDelayedInputSimulator() => new(TimeSpan.FromSeconds(_stateManager.AppConfig.AutomationConfig.AutoGuiPause));
}
