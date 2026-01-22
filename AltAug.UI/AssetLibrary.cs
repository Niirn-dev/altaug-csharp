using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AltAug.UI;

internal static class AssetLibrary
{
    public static Bitmap GetAlchemyOrbBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/alch_orb.png")));
    public static Bitmap GetAlterationOrbBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/alt_orb.png")));
    public static Bitmap GetAugmentationOrbBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/aug_orb.png")));
    public static Bitmap GetBowItemBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/bow_item.png")));
    public static Bitmap GetChaosOrbBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/chaos_orb.png")));
    public static Bitmap GetMapItemBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/map.png")));
    public static Bitmap GetScouringOrbBitmap() => new(AssetLoader.Open(new Uri("avares://AltAug.UI/Assets/scour_orb.png")));
}
