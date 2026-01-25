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

internal sealed class AutomationService(IStateManager<AppConfig> stateManager) : IAutomationService
{
    private readonly IStateManager<AppConfig> _stateManager = stateManager;

    public Option<Vec2> RecordMousePosition(int pollRate = 20, int failsafeTimeoutSeconds = 10)
    {
        InputSimulator inputSimulator = new();

        var point = Option<Vec2>.None;
        foreach (var _ in Enumerable.Range(0, pollRate * failsafeTimeoutSeconds))
        {
            if (inputSimulator.InputDeviceState.IsHardwareKeyDown(VirtualKeyCode.VK_E))
            {
                point = WindowsNativeHelper.GetCursorPosition();
                break;
            }

            if (inputSimulator.InputDeviceState.IsAnyHardwareKeyDown([VirtualKeyCode.VK_Q, VirtualKeyCode.ESCAPE]))
                break;

            Thread.Sleep(TimeSpan.FromSeconds(1.0 / pollRate));
        }

        while (inputSimulator.InputDeviceState.IsAnyHardwareKeyDown([VirtualKeyCode.VK_E, VirtualKeyCode.VK_Q, VirtualKeyCode.ESCAPE]))
            Thread.Sleep(TimeSpan.FromSeconds(1.0 / pollRate));

        return point;
    }

    public string GetItemDescription(ItemLocationParams locationParams)
    {
        GetDelayedInputSimulator()
            .Mouse
            .HoverItem(_stateManager.State, locationParams)
            .Keyboard
            .ModifiedKeyStroke([VirtualKeyCode.CONTROL, VirtualKeyCode.LMENU], VirtualKeyCode.VK_C);

        return ClipboardService.GetText() ?? string.Empty;
    }

    public void UseCurrency(CurrencyOrb orb, ItemLocationParams locationParams) => GetDelayedInputSimulator()
        .Mouse
        .HoverCurrency(_stateManager.State, orb)
        .RightButtonClick()
        .HoverItem(_stateManager.State, locationParams)
        .LeftButtonClick();

    private DelayedInputSimulator GetDelayedInputSimulator() => new(TimeSpan.FromSeconds(_stateManager.State.AutomationConfig.AutoGuiPause));
}
