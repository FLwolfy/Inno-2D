using System.Runtime.CompilerServices;

using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

using Veldrid;

namespace InnoInternal.Resource.Bridge;

internal class VeldridMaterial2D : IMaterial2D
{
    private VeldridRenderCommand m_command;
    
    private IShader m_vertexShader;
    private IShader m_fragmentShader;
    
    public IShader vertexShader => m_vertexShader;
    public IShader fragmentShader => m_fragmentShader;

    private readonly Dictionary<string, DeviceBuffer> m_uniformBuffers = new();
    private readonly Dictionary<string, ITexture2D> m_textures = new();

    private readonly GraphicsDevice m_device;
    private readonly ResourceFactory m_factory;

    private ResourceLayout? m_resourceLayout;
    private ResourceSet? m_cachedResourceSet;
    private bool m_dirty = true;

    
    public static VeldridMaterial2D LoadFromFile(IRenderCommand command, string? path)
    {
        // TODO: add true load logics
        var material2D = new VeldridMaterial2D();
        material2D.m_command = (VeldridRenderCommand)command;
        
        return material2D;
    }


    public void SetVertexShader(IShader vertex)
    {
        m_vertexShader = vertex;
        m_dirty = true;
    }

    public void SetFragmentShader(IShader fragment)
    {
        m_fragmentShader = fragment;
        m_dirty = true;
    }
    
    public void SetUniform<T>(string name, T value) where T : unmanaged
    {
        if (!m_uniformBuffers.TryGetValue(name, out var buffer))
        {
            buffer = m_factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<T>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic
            ));
            m_uniformBuffers[name] = buffer;
            m_dirty = true;
        }

        var cmd = m_command.commandList;
        cmd.UpdateBuffer(buffer, 0, value);
    }
    
    public void SetTexture(string name, ITexture2D texture)
    {
        m_textures[name] = texture;
        m_dirty = true;
    }


    public void Bind()
    {
        if (m_dirty || m_cachedResourceSet == null)
            RebuildResourceSet();

        var cmd = m_command.commandList;
        cmd.SetGraphicsResourceSet(0, m_cachedResourceSet!);
    }

    
    private void RebuildResourceSet()
    {
        // 1. 组合 layout 描述
        var layoutElems = new List<ResourceLayoutElementDescription>();
        var bindings = new List<BindableResource>();

        uint binding = 0;

        foreach (var (name, buffer) in m_uniformBuffers)
        {
            layoutElems.Add(new ResourceLayoutElementDescription(
                name, ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment
            ));
            bindings.Add(buffer);
            binding++;
        }

        foreach (var (name, tex) in m_textures)
        {
            layoutElems.Add(new ResourceLayoutElementDescription(
                name, ResourceKind.TextureReadOnly, ShaderStages.Fragment
            ));
            bindings.Add(((VeldridTexture2D)tex).textureView);
            binding++;

            // Add corresponding sampler
            layoutElems.Add(new ResourceLayoutElementDescription(
                name + "_Sampler", ResourceKind.Sampler, ShaderStages.Fragment
            ));
            
            // TODO: Add Customizable Sampler
            var sampler = m_command.graphicsDevice.ResourceFactory.CreateSampler(new SamplerDescription());
            
            bindings.Add(sampler);
            binding++;
        }

        m_resourceLayout = m_factory.CreateResourceLayout(new ResourceLayoutDescription(layoutElems.ToArray()));
        m_cachedResourceSet = m_factory.CreateResourceSet(new ResourceSetDescription(m_resourceLayout, bindings.ToArray()));
        m_dirty = false;
    }

}