namespace InnoInternal.Resource.Impl;

internal interface IMaterial : IAsset
{
    IShader shader { get; }
    
    void SetShader(IShader shader);
    void SetUniform<T>(string name, T value) where T : unmanaged;
    void SetTexture(string name, ITexture2D texture);
    void Bind();
}