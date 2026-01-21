using WindowsInput;

namespace AltAug.Application.Wrappers;

internal sealed class DelayedKeyboardSimulator(IInputSimulator inputSimulator, TimeSpan delay) : IKeyboardSimulator
{
    private readonly TimeSpan _delay = delay;
    private readonly KeyboardSimulator _defaultSimulator = new(inputSimulator);

    public IMouseSimulator Mouse { get; } = inputSimulator.Mouse;

    public IKeyboardSimulator KeyDown(VirtualKeyCode keyCode)
    {
        _defaultSimulator
            .KeyDown(keyCode)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator KeyDown(params VirtualKeyCode[] keyCodes)
    {
        _defaultSimulator
            .KeyDown(keyCodes)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator KeyPress(VirtualKeyCode keyCode)
    {
        _defaultSimulator
            .KeyPress(keyCode)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes)
    {
        _defaultSimulator
            .KeyPress(keyCodes)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator KeyUp(VirtualKeyCode keyCode)
    {
        _defaultSimulator
            .KeyUp(keyCode)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator KeyUp(params VirtualKeyCode[] keyCodes)
    {
        _defaultSimulator
            .KeyUp(keyCodes)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, IEnumerable<VirtualKeyCode> keyCodes)
    {
        _defaultSimulator
            .ModifiedKeyStroke(modifierKeyCodes, keyCodes)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode)
    {
        _defaultSimulator
            .ModifiedKeyStroke(modifierKeyCodes, keyCode)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes)
    {
        _defaultSimulator
            .ModifiedKeyStroke(modifierKey, keyCodes)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode)
    {
        _defaultSimulator
            .ModifiedKeyStroke(modifierKeyCode, keyCode)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator Sleep(int millsecondsTimeout)
    {
        _defaultSimulator
            .Sleep(millsecondsTimeout);

        return this;
    }

    public IKeyboardSimulator Sleep(TimeSpan timeout)
    {
        _defaultSimulator
            .Sleep(timeout);

        return this;
    }

    public IKeyboardSimulator TextEntry(string text)
    {
        _defaultSimulator
            .TextEntry(text)
            .Sleep(_delay);

        return this;
    }

    public IKeyboardSimulator TextEntry(char character)
    {
        _defaultSimulator
            .TextEntry(character)
            .Sleep(_delay);

        return this;
    }
}
