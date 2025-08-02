using InnoBase;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

using Microsoft.Xna.Framework.Graphics;

using Color = InnoBase.Color;
using MXColor = Microsoft.Xna.Framework.Color;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderCommand : IRenderCommand
{
    public GraphicsDevice graphicsDevice { get; private set; } = null!;
    public IRenderTarget? renderTarget { get; private set; }
    
    public Matrix viewMatrix { get; private set; } = Matrix.identity;
    public Matrix projectionMatrix { get; private set; } = Matrix.identity;

    public void Initialize(object graphicDevice)
    {
        if (graphicDevice is not GraphicsDevice device)
            throw new ArgumentException("Invalid data type. Expected 'GraphicsDevice'.", nameof(graphicDevice));

        graphicsDevice = device;
    }

    public void Begin(Matrix view, Matrix projection)
    {
        viewMatrix = view;
        projectionMatrix = projection;
    }

    public void End()
    {
        renderTarget = null;
        viewMatrix = Matrix.identity;
        projectionMatrix = Matrix.identity;
        
        graphicsDevice.SetRenderTarget(null);
    }

    public void Clear(Color color)
    {
        graphicsDevice.Clear(new MXColor(color.r, color.g, color.b, color.a));
    }

    public void DrawMesh(IMesh mesh, IMaterial material, ITexture2D? textureOverride = null)
    {
        // TODO Draw the mesh using the provided material and texture override.
    }

    public void SetRenderTarget(IRenderTarget? target)
    {
        if (target is not MonoGameRenderTarget monoGameTarget)
            throw new ArgumentException("Invalid render target type. Expected 'MonoGameRenderTarget'.", nameof(target));

        renderTarget = monoGameTarget;
        graphicsDevice.SetRenderTarget(monoGameTarget.rawRenderTarget2D);
    }

    public void SetViewPort(Vector4 viewport)
    {
        var currentViewport = graphicsDevice.Viewport;
        int targetWidth = currentViewport.Width;
        int targetHeight = currentViewport.Height;

        int x = (int)(viewport.x * targetWidth);
        int y = (int)(viewport.y * targetHeight);
        int width = (int)((viewport.z - viewport.x) * targetWidth);
        int height = (int)((viewport.w - viewport.y) * targetHeight);

        graphicsDevice.Viewport = new Viewport(x, y, width, height);
    }
}