namespace InnoInternal.Render.Impl;

public interface IResourceSet
{
    void BindUniformBuffer(int bindingIndex, IUniformBuffer uniform);
    void BindTexture(int bindingIndex, ITexture texture);
}