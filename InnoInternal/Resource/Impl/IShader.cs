namespace InnoInternal.Resource.Impl;

internal interface IShader
{
    void SetUniform<T>(string name, T value);
    void SetTexture(string name, ITexture2D texture);
    void Bind();
}