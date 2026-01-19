using Avalonia;
using Avalonia.Logging;

namespace AltAug.UI;

public static class UIServices
{
    public static void Build(string[] args, IServiceProvider services) => AppBuilder
        .Configure(() => new App(services))
        .UsePlatformDetect()
        .LogToTrace(level: LogEventLevel.Debug)
        .StartWithClassicDesktopLifetime(args);
}
