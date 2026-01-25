using AltAug.Domain.Models;

namespace AltAug.Domain.Interfaces;

public interface IStateManager<TState>
{
    TState State { get; }

    void Save();
    void Update(Func<TState, TState> updater);
}
