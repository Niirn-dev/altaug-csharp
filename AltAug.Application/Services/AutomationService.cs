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

internal sealed class AutomationService(IStateManager<AppConfig> appManager) : IAutomationService
{
    private readonly IStateManager<AppConfig> _appManager = appManager;

    public Option<Vec2> GetMousePosition() => WindowsNativeHelper.GetCursorPosition();

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
            .HoverItem(_appManager.State, locationParams)
            .Keyboard
            .ModifiedKeyStroke([VirtualKeyCode.CONTROL, VirtualKeyCode.LMENU], VirtualKeyCode.VK_C);

        return ClipboardService.GetText() ?? string.Empty;
    }

    public void UseCurrency(CurrencyOrb orb, ItemLocationParams locationParams) => GetDelayedInputSimulator()
        .Mouse
        .HoverCurrency(_appManager.State, orb)
        .RightButtonClick()
        .HoverItem(_appManager.State, locationParams)
        .LeftButtonClick();

    private DelayedScaledInputSimulator GetDelayedInputSimulator() => new(
        delay: TimeSpan.FromSeconds(_appManager.State.AutomationConfig.AutoGuiPause),
        screenResolution: _appManager.State.AutomationConfig.ScreenResolution);
}
