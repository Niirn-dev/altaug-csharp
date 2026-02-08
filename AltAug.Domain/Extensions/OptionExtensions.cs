using LanguageExt;

namespace AltAug.Domain.Extensions;

public static class OptionExtensions
{
    public static Option<T> ToOpt<T>(this T? that) => that is not null
        ? that
        : Option<T>.None;

    public static Option<string> ToOpt(this string? that, bool treatEmptyAsNone)
    {
        if (that is null || treatEmptyAsNone && string.IsNullOrEmpty(that))
            return Option<string>.None;

        return that;
    }

    public static Option<TEnum> ParseEnum<TEnum>(this string that, bool ignoreCase)
        where TEnum : struct, Enum
        => Enum.TryParse<TEnum>(that, ignoreCase, out var enumValue)
            ? enumValue
            : Option<TEnum>.None;
}
