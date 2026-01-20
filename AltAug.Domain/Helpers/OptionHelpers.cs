using LanguageExt;

using static LanguageExt.Prelude;

namespace AltAug.Domain.Helpers;

public static class OptionHelpers
{
    public static Option<TEnum> ParseEnum<TEnum>(string input, bool ignoreCase)
        where TEnum : struct, Enum
        => Enum.TryParse(input, ignoreCase, out TEnum result)
            ? result
            : None;
}
