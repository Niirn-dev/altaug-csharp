namespace AltAug.Domain.Models.StateManagers;

public sealed class RegexLibraryStateManager : StateManagerBase<RegexLibraryStore>
{
    private const string LibraryStoreFilePath = "etc/regex_store.yaml";

    private static readonly Lazy<RegexLibraryStateManager> _instance = new(() => new RegexLibraryStateManager());

    public static RegexLibraryStateManager Instance { get; } = _instance.Value;

    private RegexLibraryStateManager() : base(LibraryStoreFilePath, RegexLibraryStore.DefaultInstance)
    { }
}
