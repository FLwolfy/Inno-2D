using InnoInternal.Resource.Impl;

namespace InnoEngine.Resource.AssetType;

public class Texture2D : InnoAsset
{
    internal ITexture2D texture2DImpl { get; private set; } = null!;

    public int width => texture2DImpl.width;
    public int height => texture2DImpl.height;
    
    internal Texture2D() {}

    protected override void OnLoad(string? pathStr)
    {
        texture2DImpl = AssetManager.LoadImplFromLoaders<ITexture2D>(pathStr);
    }
}