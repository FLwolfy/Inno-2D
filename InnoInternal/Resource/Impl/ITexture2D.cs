namespace InnoInternal.Resource.Impl;

internal interface ITexture2D : IAsset
{
    uint width { get; }
    uint height { get; }
}