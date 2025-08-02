using InnoInternal.ImGui.Bridge;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.Render.Bridge;

internal class MonoGameRenderAPI : IRenderAPI
{
    public IRenderer2D renderer2D { get; } = new MonoGameRenderer2D();
    public IImGuiRenderer rendererGUI { get; } = new ImGuiNETMonoGameRenderer();
    public IRenderContext renderContext { get; } = new MonoGameRenderContext();
    public IAssetLoader renderAssetLoader { get; } = new MonoGameAssetLoader();
    
    public void Initialize(object graphicDevice, object windowHolder)
    {
        if (graphicDevice is not GraphicsDevice) 
            throw new ArgumentException("Invalid data type. Expected 'GraphicsDevice'.", nameof(graphicDevice));
        if (windowHolder is not Game)
            throw new ArgumentException("Invalid data type. Expected 'Game'.", nameof(windowHolder));
        
        var command = new MonoGameRenderCommand();
        
        renderer2D.Initialize(command);
        rendererGUI.Initialize(graphicDevice, windowHolder);
        renderContext.Initialize(command);
        renderAssetLoader.Initialize(command);
    }
}