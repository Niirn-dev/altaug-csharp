using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Logging;

public sealed class TextBoxLoggerProvider(TextBox textBox) : ILoggerProvider
{
    private readonly TextBox _textBox = textBox;

    public ILogger CreateLogger(string categoryName) => new TextBoxLogger(_textBox, categoryName);

    public void Dispose()
    {
    }
}

internal sealed class TextBoxLogger(TextBox textBox, string categoryName) : ILogger
{
    private readonly TextBox _textBox = textBox;
    private readonly string _categoryName = categoryName;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = $"[{logLevel}] {_categoryName}: {formatter(state, exception)}{Environment.NewLine}";

        Dispatcher.UIThread.Post(() => _textBox.Text += message);
    }
}
