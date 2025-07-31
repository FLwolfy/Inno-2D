namespace InnoInternal.Resource.Impl;

internal interface IMaterial2D
{
    IShader vertexShader { get; }
    IShader fragmentShader { get; }
    
    void SetVertexShader(IShader vertexShader);
    void SetFragmentShader(IShader fragmentShader);
    
    // TODO: Add new Dictionary to show shader uniform settings
    
    void SetUniform<T>(string name, T value);
    void SetTexture(string name, ITexture2D texture);
    void Bind();
}