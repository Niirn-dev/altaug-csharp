using AltAug.Domain.Models;

namespace AltAug.Domain.Interfaces;

public interface IFilter
{
    bool IsMatch(ItemInfo item);
}
