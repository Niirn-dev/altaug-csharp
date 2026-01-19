using AltAug.Domain.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AltAug.Domain;

public sealed class StateManager
{
    private const string ConfigFilePath = "config.yaml";

    private static readonly Lazy<StateManager> _instance = new(() => new StateManager());
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public static StateManager Instance { get; } = _instance.Value;
    public AppConfig AppConfig { get; private set; }

    private StateManager()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        if (File.Exists(ConfigFilePath))
        {
            var yaml = File.ReadAllText(ConfigFilePath);
            AppConfig = _deserializer.Deserialize<AppConfig>(yaml);
        }
        else
        {
            AppConfig = AppConfig.DefaultInstance;
        }
    }

    public void Save()
    {
        var yaml = _serializer.Serialize(AppConfig);
        File.WriteAllText(ConfigFilePath, yaml);
    }

    public void Update(Func<AppConfig, AppConfig> updater)
    {
        AppConfig = updater(AppConfig);
    }
}
