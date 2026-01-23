using LanguageExt;

namespace AltAug.Domain.Extensions;

public static class OptionExtensions
{
    public static Option<T> ToOpt<T>(this T? that) => that is not null
        ? that
        : Option<T>.None;

    public static Option<TEnum> ParseEnum<TEnum>(this string that)
        where TEnum : struct, Enum
        => Enum.TryParse<TEnum>(that, out var enumValue)
            ? enumValue
            : Option<TEnum>.None;
}
