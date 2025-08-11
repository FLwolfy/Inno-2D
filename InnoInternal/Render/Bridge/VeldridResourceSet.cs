using InnoInternal.Render.Impl;

namespace InnoInternal.Render.Bridge;

public class VeldridResourceSet : IResourceSet
{
    public void BindUniformBuffer(int bindingIndex, IUniformBuffer uniform)
    {
        throw new NotImplementedException();
    }

    public void BindTexture(int bindingIndex, ITexture texture)
    {
        throw new NotImplementedException();
    }
}