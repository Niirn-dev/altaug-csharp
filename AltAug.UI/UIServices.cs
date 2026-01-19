using Avalonia;
using Avalonia.Logging;

namespace AltAug.UI;

public static class UIServices
{
    public static void Build(string[] args) => AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .LogToTrace(level: LogEventLevel.Debug)
        .StartWithClassicDesktopLifetime(args);
}
