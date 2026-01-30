using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Logging;

public sealed class TextBoxLoggerProvider(TextBox textBox, bool useMinimalFormat) : ILoggerProvider
{
    private readonly TextBox _textBox = textBox;

    public ILogger CreateLogger(string categoryName) => new TextBoxLogger(_textBox, categoryName, useMinimalFormat);

    public void Dispose()
    {
    }
}

internal sealed class TextBoxLogger(TextBox textBox, string categoryName, bool useMinimalFormat) : ILogger
{
    private const string FullMessageFormat = "[{0}] {1}: {2}{3}";
    private const string MinimalMessageFormat = "{0}{1}";

    private readonly TextBox _textBox = textBox;
    private readonly string _categoryName = categoryName;
    private readonly bool _useMinimalFormat = useMinimalFormat;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = _useMinimalFormat
            ? string.Format(MinimalMessageFormat, formatter(state, exception), Environment.NewLine)
            : string.Format(FullMessageFormat, logLevel, _categoryName, formatter(state, exception), Environment.NewLine);

        Dispatcher.UIThread.Post(() => _textBox.Text += message);
    }
}
