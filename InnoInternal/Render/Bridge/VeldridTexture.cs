using InnoInternal.Render.Impl;
using Veldrid;

using InnoTEXDescription = InnoInternal.Render.Impl.TextureDescription;
using VeldridTEXDescription = Veldrid.TextureDescription;

using VTexture = Veldrid.Texture;

namespace InnoInternal.Render.Bridge;

internal class VeldridTexture : ITexture
{
    private readonly GraphicsDevice m_graphicsDevice;
    
    public int width { get; }
    public int height { get; }
    
    internal readonly Texture inner;

    public VeldridTexture(GraphicsDevice graphicsDevice, Texture inner)
    {
        m_graphicsDevice = graphicsDevice;
        
        this.inner = inner;
        width = (int)inner.Width;
        height = (int)inner.Height;
    }

    public static VeldridTexture Create(GraphicsDevice graphicsDevice, InnoTEXDescription desc)
    {
        VTexture vTexture = graphicsDevice.ResourceFactory.CreateTexture(ToVeldridTEXDesc(desc));
        
        return new VeldridTexture(graphicsDevice, vTexture);
    }

    private static VeldridTEXDescription ToVeldridTEXDesc(InnoTEXDescription desc)
    {
        var width = (uint) desc.width;
        var height = (uint) desc.height;

        return new VeldridTEXDescription
        {
            Width = width,
            Height = height,
            Depth = 1,
            MipLevels = 1,
            ArrayLayers = 1,
            Format = PixelFormat.R8_G8_B8_A8_UNorm,
            Usage = TextureUsage.Sampled | TextureUsage.Storage,
            Type = TextureType.Texture2D
        };
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}