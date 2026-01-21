using WindowsInput;

namespace AltAug.Application.Wrappers;

internal sealed class DelayedMouseSimulator(IInputSimulator inputSimulator, TimeSpan delay) : IMouseSimulator
{
    private readonly TimeSpan _delay = delay;
    private readonly MouseSimulator _defaultSimulator = new(inputSimulator);

    public int MouseWheelClickSize { get => _defaultSimulator.MouseWheelClickSize; set => _defaultSimulator.MouseWheelClickSize = value; }

    public IKeyboardSimulator Keyboard { get; } = inputSimulator.Keyboard;

    public IMouseSimulator MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
    {
        _defaultSimulator
            .MoveMouseBy(pixelDeltaX, pixelDeltaY)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator MoveMouseTo(double absoluteX, double absoluteY)
    {
        _defaultSimulator
            .MoveMouseTo(absoluteX, absoluteY)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator MoveMouseToPositionOnVirtualDesktop(double absoluteX, double absoluteY)
    {
        _defaultSimulator
            .MoveMouseToPositionOnVirtualDesktop(absoluteX, absoluteY)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator LeftButtonDown()
    {
        _defaultSimulator
            .LeftButtonDown()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator LeftButtonUp()
    {
        _defaultSimulator
            .LeftButtonUp()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator LeftButtonClick()
    {
        _defaultSimulator
            .LeftButtonClick()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator LeftButtonDoubleClick()
    {
        _defaultSimulator
            .LeftButtonDoubleClick()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator MiddleButtonDown()
    {
        _defaultSimulator
            .MiddleButtonDown()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator MiddleButtonUp()
    {
        _defaultSimulator
            .MiddleButtonUp()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator MiddleButtonClick()
    {
        _defaultSimulator
            .MiddleButtonClick()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator MiddleButtonDoubleClick()
    {
        _defaultSimulator
            .MiddleButtonDoubleClick()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator RightButtonDown()
    {
        _defaultSimulator
            .RightButtonDown()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator RightButtonUp()
    {
        _defaultSimulator
            .RightButtonUp()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator RightButtonClick()
    {
        _defaultSimulator
            .RightButtonClick()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator RightButtonDoubleClick()
    {
        _defaultSimulator
            .RightButtonDoubleClick()
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator XButtonDown(int buttonId)
    {
        _defaultSimulator
            .XButtonDown(buttonId)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator XButtonUp(int buttonId)
    {
        _defaultSimulator
            .XButtonUp(buttonId)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator XButtonClick(int buttonId)
    {
        _defaultSimulator
            .XButtonClick(buttonId)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator XButtonDoubleClick(int buttonId)
    {
        _defaultSimulator
            .XButtonDoubleClick(buttonId)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator VerticalScroll(int scrollAmountInClicks)
    {
        _defaultSimulator
            .VerticalScroll(scrollAmountInClicks)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator HorizontalScroll(int scrollAmountInClicks)
    {
        _defaultSimulator
            .HorizontalScroll(scrollAmountInClicks)
            .Sleep(_delay);

        return this;
    }

    public IMouseSimulator Sleep(int millsecondsTimeout)
    {
        _defaultSimulator
            .Sleep(millsecondsTimeout);

        return this;
    }

    public IMouseSimulator Sleep(TimeSpan timeout)
    {
        _defaultSimulator
            .Sleep(timeout);

        return this;
    }
}
