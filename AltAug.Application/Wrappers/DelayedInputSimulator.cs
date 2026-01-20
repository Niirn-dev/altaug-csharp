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

internal sealed class DelayedKeyboardSimulator(IInputSimulator inputSimulator, TimeSpan delay) : KeyboardSimulator(inputSimulator)
{
    private readonly TimeSpan _delay = delay;

    public new IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode) => base
        .ModifiedKeyStroke(modifierKeyCodes, keyCode)
        .Sleep(_delay);
}

internal sealed class DelayedMouseSimulator(IInputSimulator inputSimulator, TimeSpan delay) : MouseSimulator(inputSimulator)
{
    private readonly TimeSpan _delay = delay;

    public new IMouseSimulator MoveMouseTo(double absoluteX, double absoluteY) => base
        .MoveMouseTo(absoluteX, absoluteY)
        .Sleep(_delay);

    public new IMouseSimulator LeftButtonClick() => base
        .LeftButtonClick()
        .Sleep(_delay);

    public new IMouseSimulator RightButtonClick() => base
        .RightButtonClick()
        .Sleep(_delay);
}

