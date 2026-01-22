using AltAug.Application.Extensions;
using AltAug.Application.Helpers;
using AltAug.Application.Wrappers;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.Domain.Models.Enums;
using LanguageExt;
using TextCopy;
using WindowsInput;

namespace AltAug.Application.Services;

internal sealed class AutomationService(IStateManager stateManager) : IAutomationService
{
    private readonly IStateManager _stateManager = stateManager;

    public Option<Vec2> RecordMousePosition(int pollRate = 20, int failsafeTimeoutSeconds = 10)
    {
        InputSimulator inputSimulator = new();

        foreach (var _ in Enumerable.Range(0, pollRate * failsafeTimeoutSeconds))
        {
            if (inputSimulator.InputDeviceState.IsHardwareKeyDown(VirtualKeyCode.VK_E))
                return WindowsNativeHelper.GetCursorPosition();

            if (inputSimulator.InputDeviceState.IsAnyHardwareKeyDown([VirtualKeyCode.VK_Q, VirtualKeyCode.ESCAPE]))
                return Option<Vec2>.None;

            Thread.Sleep(TimeSpan.FromSeconds(1.0 / pollRate));
        }

        return Option<Vec2>.None;
    }

    public string GetItemDescription(ItemLocationParams locationParams)
    {
        GetDelayedInputSimulator()
            .Mouse
            .HoverItem(_stateManager.AppConfig, locationParams)
            .Keyboard
            .ModifiedKeyStroke([VirtualKeyCode.CONTROL, VirtualKeyCode.LMENU], VirtualKeyCode.VK_C);

        return ClipboardService.GetText() ?? string.Empty;
    }

    public void UseCurrency(CurrencyOrb orb, ItemLocationParams locationParams) => GetDelayedInputSimulator()
        .Mouse
        .HoverCurrency(_stateManager.AppConfig, orb)
        .RightButtonClick()
        .HoverItem(_stateManager.AppConfig, locationParams)
        .LeftButtonClick();

    private DelayedInputSimulator GetDelayedInputSimulator() => new(TimeSpan.FromSeconds(_stateManager.AppConfig.AutomationConfig.AutoGuiPause));
}
