using Inno.Core.ECS;
using Inno.Core.Layers;
using Inno.Graphics;
using Inno.Platform.Graphics;
using Inno.Runtime.RenderPasses;

namespace Inno.Runtime.Core;

public class GameLayer : Layer
{
    private readonly RenderContext m_renderContext;

    public GameLayer() : base("GameLayer")
    {
        m_renderContext = new RenderContext();
        
        // Passes
        m_renderContext.passStack.PushPass(new ClearScreenPass());
        m_renderContext.passStack.PushPass(new SpriteRenderPass());
    }
    
    public override void OnAttach()
    {
        SceneManager.BeginRuntime();
    }

    public override void OnDetach()
    {
        m_renderContext.Dispose();
    }

    public override void OnUpdate()
    {
        SceneManager.UpdateActiveScene();
    }

    public override void OnRender()
    {
        // Get Camera
        var camera = SceneManager.GetActiveScene()?.GetMainCamera();
        if (camera == null) { return; }
        
        // Render Pipeline
        EnsureSceneRenderTarget();
        
        // TODO: Use Renderer2D Blit
        m_renderContext.renderer2D.BeginFrame(camera.viewMatrix * camera.projectionMatrix, camera.aspectRatio, RenderTargetPool.GetMain());
        m_renderContext.passStack.OnRender(m_renderContext);
        m_renderContext.renderer2D.EndFrame();
    }
    
    private void EnsureSceneRenderTarget()
    {
        if (RenderTargetPool.Get("scene") == null)
        {
            var renderTexDesc = new TextureDescription
            {
                format = PixelFormat.B8_G8_R8_A8_UNorm,
                usage = TextureUsage.RenderTarget | TextureUsage.Sampled,
                dimension = TextureDimension.Texture2D
            };
            
            var depthTexDesc = new TextureDescription
            {
                format = PixelFormat.D32_Float_S8_UInt,
                usage = TextureUsage.DepthStencil,
                dimension = TextureDimension.Texture2D
            };
            
            var renderTargetDesc = new FrameBufferDescription
            {
                depthAttachmentDescription = depthTexDesc,
                colorAttachmentDescriptions = [renderTexDesc]
            };
            
            RenderTargetPool.Create("scene", renderTargetDesc);
        }
    }
}