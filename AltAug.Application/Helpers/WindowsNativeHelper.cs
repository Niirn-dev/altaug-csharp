using System.Runtime.InteropServices;
using AltAug.Domain.Models;
using LanguageExt;

namespace AltAug.Application.Helpers;

internal static class WindowsNativeHelper
{
    public static Option<Vec2> GetCursorPosition() => GetCursorPos(out var p)
        ? new Vec2(p.X, p.Y)
        : Option<Vec2>.None;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    private readonly record struct POINT(int X, int Y);
}
