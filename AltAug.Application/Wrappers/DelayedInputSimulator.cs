using WindowsInput;

namespace AltAug.Application.Wrappers;

internal sealed class DelayedInputSimulator : IInputSimulator
{
    public IKeyboardSimulator Keyboard { get; init; }
    public IMouseSimulator Mouse { get; init; }
    public IInputDeviceStateAdaptor InputDeviceState { get; init; }

    public DelayedInputSimulator(TimeSpan delay)
    {
        Keyboard = new DelayedKeyboardSimulator(this, delay);
        Mouse = new DelayedMouseSimulator(this, delay);
        InputDeviceState = new WindowsInputDeviceStateAdaptor();
    }
}
