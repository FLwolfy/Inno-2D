namespace Inno.Platform.Graphics;

public struct ResourceSetBinding
{
    public ShaderStage shaderStages;
    
    public IUniformBuffer[] uniformBuffers;
    // public ITexture[] textures;
}

public interface IResourceSet : IDisposable;