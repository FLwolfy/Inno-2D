namespace InnoInternal.Render.Impl;

public struct ResourceSetDescription
{
    public IUniformBuffer uniformBuffer;
    public ITexture texture;
    public IShader shader;
}

public interface IResourceSet;