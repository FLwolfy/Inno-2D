using InnoInternal.Render.Impl;

using Veldrid;

namespace InnoInternal.Render.Bridge;

internal class VeldridResourceSet : IResourceSet
{
    internal ResourceSet inner { get; }

    public VeldridResourceSet(ResourceSet resourceSet)
    {
        inner = resourceSet;
    }
}