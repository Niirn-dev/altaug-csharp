namespace AltAug.Domain.Models.StateManagers;

public sealed class ApplicationStateManager : StateManagerBase<AppConfig>
{
    private const string ConfigFilePath = "etc/config.yaml";

    private static readonly Lazy<ApplicationStateManager> _instance = new(() => new ApplicationStateManager());

    public static ApplicationStateManager Instance { get; } = _instance.Value;

    private ApplicationStateManager() : base(ConfigFilePath, AppConfig.DefaultInstance)
    { }
}
