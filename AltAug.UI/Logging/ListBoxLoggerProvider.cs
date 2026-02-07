using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Logging;

public sealed class ListBoxLoggerProvider(ListBox logPresenter) : ILoggerProvider
{
    private readonly ListBox _logPresenter = logPresenter;

    public ILogger CreateLogger(string categoryName) => new ListBoxLogger(_logPresenter);

    public void Dispose()
    {
    }
}

internal sealed class ListBoxLogger(ListBox logPresenter) : ILogger
{
    private readonly ListBox _logPresenter = logPresenter;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Dispatcher.UIThread.Post(() => _logPresenter.Items.Add(formatter(state, exception)));
    }
}
