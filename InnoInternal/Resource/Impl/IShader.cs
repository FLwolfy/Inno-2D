namespace InnoInternal.Resource.Impl;

internal interface IShader : IAsset
{
    void SetTexture(string name, ITexture2D texture);
    void Apply();
}