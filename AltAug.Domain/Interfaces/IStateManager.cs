using AltAug.Domain.Models;

namespace AltAug.Domain.Interfaces;

public interface IStateManager
{
    AppConfig AppConfig { get; }

    void Save();
    void Update(Func<AppConfig, AppConfig> updater);
}
