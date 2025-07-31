namespace InnoInternal.Resource.Impl;

internal interface IMaterial2D
{
    IShader vertexShader { get; }
    IShader fragmentShader { get; }
    
    void SetVertexShader(IShader vertex);
    void SetFragmentShader(IShader fragment);
    
    // TODO: Add new Dictionary to show shader uniform settings
    
    void SetUniform<T>(string name, T value) where T : unmanaged;
    void SetTexture(string name, ITexture2D texture);
    void Bind();
}