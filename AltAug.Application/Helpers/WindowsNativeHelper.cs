using System.Runtime.InteropServices;
using AltAug.Domain.Models;
using LanguageExt;

namespace AltAug.Application.Helpers;

internal static class WindowsNativeHelper
{
    public static Option<Point> GetCursorPosition() => GetCursorPos(out var p)
        ? new Point(p.X, p.Y)
        : Option<Point>.None;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    private readonly record struct POINT(int X, int Y);
}
