using AltAug.Domain.Models;
using WindowsInput;

namespace AltAug.Application.Wrappers;

internal sealed class DelayedScaledInputSimulator : IInputSimulator
{
    public IKeyboardSimulator Keyboard { get; init; }
    public IMouseSimulator Mouse { get; init; }
    public IInputDeviceStateAdaptor InputDeviceState { get; init; }

    public DelayedScaledInputSimulator(TimeSpan delay, Vec2 screenResolution)
    {
        Keyboard = new DelayedKeyboardSimulator(this, delay);
        Mouse = new DelayedScaledMouseSimulator(this, delay, screenResolution);
        InputDeviceState = new WindowsInputDeviceStateAdaptor();
    }
}
