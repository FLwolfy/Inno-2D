using Inno.Core.Math;
using Inno.Graphics.Resources;
using Inno.Platform.Graphics;

namespace Inno.Graphics;

public class Renderer2D : IDisposable
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly ICommandList m_commandList;
    
    // Render Info 
    private IFrameBuffer m_currentFrameBuffer = null!;
    private Matrix m_currentViewProjection;

    // Quad Resources
    private GraphicsResource m_quadOpaqueResources = null!;
    private GraphicsResource m_quadAlphaResources = null!;

    public Renderer2D(IGraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
        m_commandList = graphicsDevice.CreateCommandList();
    }
    
    public void LoadResources()
    {
        CreateSolidQuadResources();
    }

    private void CreateSolidQuadResources()
    {
        // Mesh
        var mesh = new Mesh("Quad");
        mesh.renderState = new MeshRenderState
        {
            topology = PrimitiveTopology.TriangleList
        };
        mesh.SetAttribute("Position", new Vector3[]
        {
            new(-1.0f,  1.0f, 0f),
            new( 1.0f,  1.0f, 0f),
            new(-1.0f, -1.0f, 0f),
            new( 1.0f, -1.0f, 0f)
        });
        mesh.SetIndices([
            0, 1, 2,
            2, 1, 3
        ]);
        
        // Opaque Material
        var opaqueMat = new Material("QuadOpaque");
        opaqueMat.renderState = new MaterialRenderState
        {
            depthStencilState = DepthStencilState.DepthOnlyLessEqual
        };
        opaqueMat.shaders = new ShaderProgram();
        opaqueMat.shaders.Add(ShaderLibrary.LoadEmbeddedShader("SolidQuad.vert"));
        opaqueMat.shaders.Add(ShaderLibrary.LoadEmbeddedShader("SolidQuad.frag"));
        
        // Alpha Material
        var alphaMat = new Material("QuadAlpha");
        alphaMat.renderState = new MaterialRenderState
        {
            depthStencilState = DepthStencilState.DepthReadOnlyLessEqual
        };
        alphaMat.shaders = new ShaderProgram();
        alphaMat.shaders.Add(ShaderLibrary.LoadEmbeddedShader("SolidQuad.vert"));
        alphaMat.shaders.Add(ShaderLibrary.LoadEmbeddedShader("SolidQuad.frag"));
        
        // Opaque Resource
        m_quadOpaqueResources = new GraphicsResource(mesh, [opaqueMat]);
        m_quadOpaqueResources.RegisterPerObjectUniform("MVP", typeof(Matrix));
        m_quadOpaqueResources.RegisterPerObjectUniform("Color", typeof(Color));
        m_quadOpaqueResources.Create(m_graphicsDevice);
        
        // Alpha Resource 
        m_quadAlphaResources = new GraphicsResource(mesh, [alphaMat]);
        m_quadAlphaResources.RegisterPerObjectUniform("MVP", typeof(Matrix));
        m_quadAlphaResources.RegisterPerObjectUniform("Color", typeof(Color));
        m_quadAlphaResources.Create(m_graphicsDevice);
    }
    
    public void DrawQuad(Matrix transform, Color color)
    {
        var mvp = transform * m_currentViewProjection;

        if (MathHelper.AlmostEquals(color.a, 1.0f))
        {
            m_quadOpaqueResources.UpdatePerObjectUniform(m_commandList, "MVP", mvp);
            m_quadOpaqueResources.UpdatePerObjectUniform(m_commandList, "Color", color);
            m_quadOpaqueResources.ApplyAll(m_commandList);
        }
        else
        {
            m_quadAlphaResources.UpdatePerObjectUniform(m_commandList, "MVP", mvp);
            m_quadAlphaResources.UpdatePerObjectUniform(m_commandList, "Color", color);
            m_quadAlphaResources.ApplyAll(m_commandList);
        }
    }
    
    public void ClearColor(Color color)
    {
        var mvp = Matrix.identity;
        
        m_quadAlphaResources.UpdatePerObjectUniform(m_commandList, "MVP", mvp);
        m_quadAlphaResources.UpdatePerObjectUniform(m_commandList, "Color", color);
        m_quadAlphaResources.ApplyAll(m_commandList);
    }

    public void BeginFrame(Matrix viewProjectionMatrix, float? aspectRatio, IFrameBuffer target)
    {
        m_currentFrameBuffer = target;
        m_currentViewProjection = viewProjectionMatrix;
        
        m_commandList.Begin();
        m_commandList.SetFrameBuffer(m_currentFrameBuffer);
        m_commandList.ClearColor(Color.BLACK);

        if (aspectRatio.HasValue)
        {
            float targetWidth = m_currentFrameBuffer.width;
            float targetHeight = m_currentFrameBuffer.height;

            float screenAspect = targetWidth / targetHeight;
            float sourceAspect = aspectRatio.Value;

            Rect viewportRect;

            // Left Right
            if (screenAspect > sourceAspect)
            {
                float newWidth = targetHeight * sourceAspect;
                float xOffset = (targetWidth - newWidth) / 2f;
                viewportRect = new Rect((int)xOffset, 0, (int)newWidth, (int)targetHeight);
            }
        
            // Top Bottom
            else
            {
                float newHeight = targetWidth / sourceAspect;
                float yOffset = (targetHeight - newHeight) / 2f;
                viewportRect = new Rect(0, (int)yOffset, (int)targetWidth, (int)newHeight);
            }

            m_commandList.SetViewPort(0, viewportRect);
            m_commandList.SetScissorRect(0, viewportRect);
        }

        if (m_currentFrameBuffer.GetDepthAttachment() != null)
        {
            m_commandList.ClearDepth(1.0f);
        }
    }

    public void EndFrame()
    {
        m_commandList.End();
        m_graphicsDevice.Submit(m_commandList);
    }

    public void Dispose()
    {
        m_commandList.Dispose();
        
        // Quad
        m_quadOpaqueResources.Dispose();
        m_quadAlphaResources.Dispose();
    }
}
