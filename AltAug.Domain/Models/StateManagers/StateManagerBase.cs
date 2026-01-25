using AltAug.Domain.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AltAug.Domain.Models.StateManagers;

public abstract class StateManagerBase<TState> : IStateManager<TState>
{
    private readonly string _stateFilePath;
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public TState State { get; private set; }

    protected StateManagerBase(string stateFilePath, TState defaultState)
    {
        _stateFilePath = stateFilePath;

        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        if (File.Exists(_stateFilePath))
        {
            var yaml = File.ReadAllText(_stateFilePath);
            State = _deserializer.Deserialize<TState>(yaml);
        }
        else
        {
            State = defaultState;
        }
    }

    public void Save()
    {
        var yaml = _serializer.Serialize(State);
        File.WriteAllText(_stateFilePath, yaml);
    }

    public void Update(Func<TState, TState> updater) => State = updater(State);
}
