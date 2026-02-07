namespace AltAug.Domain.Models.StateManagers;

public sealed class ApplicationStateManager : StateManagerBase<AppConfig>
{
    private const string ConfigFilePath = "config.yaml";

    private static readonly Lazy<ApplicationStateManager> _instance = new(() => new ApplicationStateManager());

    public static ApplicationStateManager Instance { get; } = _instance.Value;

    private ApplicationStateManager() : base(ConfigFilePath, AppConfig.DefaultInstance)
    { }

    protected override AppConfig Sanitize(AppConfig state) => state with
    {
        MainWindowConfig = state.MainWindowConfig with
        {
            Height = Math.Max(state.MainWindowConfig.Height, MainWindowConfig.DefaultHeight),
            Width = Math.Max(state.MainWindowConfig.Width, MainWindowConfig.DefaultWidth),
        },
    };
}
