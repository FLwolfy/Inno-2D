using InnoInternal.Render.Impl;

using Veldrid;
using VeldridRSDescription = Veldrid.ResourceSetDescription;

namespace InnoInternal.Render.Bridge;

internal class VeldridResourceSet : IResourceSet
{
    private readonly GraphicsDevice m_graphicsDevice;

    internal ResourceSet inner { get; }

    public VeldridResourceSet(GraphicsDevice graphicsDevice, ResourceSetBinding binding)
    {
        m_graphicsDevice = graphicsDevice;

        var innerDescription = ToVeldridRSDesc(binding);
        inner = m_graphicsDevice.ResourceFactory.CreateResourceSet(ref innerDescription);
    }
    
    private VeldridRSDescription ToVeldridRSDesc(ResourceSetBinding binding)
    {
        var uniformBuffers = binding.uniformBuffers.Length > 0
            ? binding.uniformBuffers
                .Select(ub => ((VeldridUniformBuffer)ub).inner)
                .ToArray()
            : [];

        var boundResources = new List<BindableResource>();
        boundResources.AddRange(uniformBuffers);
        
        // TODO: Add support for other resource types (e.g., textures, samplers) here.

        return new VeldridRSDescription
        {
            Layout = m_graphicsDevice.ResourceFactory.CreateResourceLayout(GenerateResourceLayoutFromBinding(binding)),
            BoundResources = boundResources.ToArray()
        };
    }
    
    internal static ResourceLayoutDescription GenerateResourceLayoutFromBinding(ResourceSetBinding b)
    {
        var elements = new List<ResourceLayoutElementDescription>();
        ShaderStages stages = VeldridShader.ToVeldridShaderStage(b.shaderStages);

        for (int i = 0; i < b.uniformBuffers.Length; i++)
        {
            elements.Add(new ResourceLayoutElementDescription(
                b.uniformBuffers[i].bufferName,
                ResourceKind.UniformBuffer, 
                stages
            ));
            
        }
        
        // TODO: Add support for other resource types (e.g., textures, samplers) here.

        return new ResourceLayoutDescription(elements.ToArray());
    }
    
    public void Dispose()
    {
        inner.Dispose();
    }
}