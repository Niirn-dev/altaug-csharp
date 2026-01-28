using AltAug.Domain.Interfaces;
using AltAug.Domain.Models.Filters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AltAug.Domain.Models.StateManagers;

public abstract class StateManagerBase<TState> : IStateManager<TState>
{
    private readonly FileInfo _stateFile;
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public TState State { get; private set; }

    protected StateManagerBase(string stateFilePath, TState defaultState)
    {
        _stateFile = new(stateFilePath);

        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTagMapping("!AffixFilterParameters", typeof(AffixFilterParameters))
            .WithTagMapping("!OpenPrefixFilterParameters", typeof(OpenPrefixFilterParameters))
            .WithTagMapping("!OpenSuffixFilterParameters", typeof(OpenSuffixFilterParameters))
            .WithTagMapping("!RegexFilterParameters", typeof(RegexFilterParameters))
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTagMapping("!AffixFilterParameters", typeof(AffixFilterParameters))
            .WithTagMapping("!OpenPrefixFilterParameters", typeof(OpenPrefixFilterParameters))
            .WithTagMapping("!OpenSuffixFilterParameters", typeof(OpenSuffixFilterParameters))
            .WithTagMapping("!RegexFilterParameters", typeof(RegexFilterParameters))
            .Build();

        if (_stateFile.Exists)
        {
            try
            {
                var yaml = File.ReadAllText(_stateFile.FullName);
                State = _deserializer.Deserialize<TState>(yaml);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed while deserializing existing state file. Continuing using the default state. {0}", ex);
                State = defaultState;
            }
        }
        else
        {
            State = defaultState;
        }
    }

    public void Save()
    {
        _stateFile.Directory?.Create();

        var yaml = _serializer.Serialize(State);
        File.WriteAllText(_stateFile.FullName, yaml);
    }

    public void Update(Func<TState, TState> updater) => State = updater(State);
}
