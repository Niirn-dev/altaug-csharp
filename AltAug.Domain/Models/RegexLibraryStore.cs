namespace AltAug.Domain.Models;

public readonly record struct RegexLibraryStore(
    Dictionary<
        string,
        Dictionary<
            string,
            Dictionary<string, string>>> TypeToBaseToNameToValue)
{
    public static RegexLibraryStore DefaultInstance { get; } = new(
        new()
        {
            [string.Empty] = new()
            {
                [string.Empty] = new()
                {
                    [string.Empty] = string.Empty
                },
            },
        }
    );
}
