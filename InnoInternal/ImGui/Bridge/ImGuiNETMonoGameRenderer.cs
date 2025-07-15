using ImGuiNET;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Bridge;
using InnoInternal.Resource.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InnoInternal.ImGui.Bridge;

internal class ImGuiNETMonoGameRenderer : IDisposable, IImGuiRenderer
{
    public IImGuiContext context => m_context;
    private readonly IImGuiContext m_context;
    
    private GraphicsDevice graphicsDevice => MonoGameRenderAPI.graphicsDevice;
    
    private DynamicVertexBuffer m_vertexBuffer;
    private DynamicIndexBuffer m_indexBuffer;

    private readonly VertexDeclaration m_vertexDeclaration =
        new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

    private int m_vertexBufferSize = 5000;
    private int m_indexBufferSize = 10000;

    private readonly Dictionary<IntPtr, Texture2D> m_loadedTextures = new();
    private IntPtr m_lastTextureId = (IntPtr)1;

    private BasicEffect m_effect;
    private RasterizerState m_rasterizerState;
    private BlendState m_blendState;
    private DepthStencilState m_depthStencilState;

    private Matrix m_projectionMatrix;

    public ImGuiNETMonoGameRenderer()
    {
        m_context = new ImGuiNETContext(this);
        ImGuiNET.ImGui.CreateContext();
        CreateDeviceResources();
    }

    private void CreateDeviceResources()
    {
        m_vertexBuffer = new DynamicVertexBuffer(graphicsDevice, m_vertexDeclaration, m_vertexBufferSize, BufferUsage.None);
        m_indexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, m_indexBufferSize, BufferUsage.None);

        m_effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            TextureEnabled = true,
            Projection = Matrix.CreateOrthographicOffCenter(0f, graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height, 0f, 0f, 1f)
        };

        m_projectionMatrix = m_effect.Projection;

        m_rasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        m_blendState = BlendState.NonPremultiplied;
        m_depthStencilState = DepthStencilState.None;
    }

    public void Initialize()
    {
        // 获取 ImGui IO 对象
        var io = ImGuiNET.ImGui.GetIO();

        // 加载默认字体（也可以自定义字体）
        io.Fonts.AddFontDefault();

        // 生成字体纹理的 RGBA 数据指针和尺寸
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

        // 创建 MonoGame 纹理对象，上传字体纹理数据
        var fontTexture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);

        // 将指针数据复制到 byte 数组中
        byte[] pixelData = new byte[width * height * bytesPerPixel];
        System.Runtime.InteropServices.Marshal.Copy(pixels, pixelData, 0, pixelData.Length);

        // 设置纹理数据
        fontTexture.SetData(pixelData);

        // 绑定字体纹理到 ImGui 的 TextureId 映射
        IntPtr fontTexId = BindTexture(fontTexture);
        io.Fonts.SetTexID(fontTexId);

        // 清除 CPU 端字体数据，节省内存
        io.Fonts.ClearTexData();

        // 你可以设置更多 ImGui IO 配置项，比如io.ConfigFlags |= ImGuiConfigFlags.DockingEnable等
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.ViewportsEnable;

        // 设置投影矩阵，更新效果（确保窗口大小正确）
        SetPerFrameData();
    }


    public void BeginFrame()
    {
        var io = ImGuiNET.ImGui.GetIO();
        
        // 这里用 MonoGame GraphicsDevice 的 Viewport 尺寸设置
        io.DisplaySize = new System.Numerics.Vector2(
            graphicsDevice.Viewport.Width,
            graphicsDevice.Viewport.Height);

        io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);
        
        ImGuiNET.ImGui.NewFrame();
    }

    public void EndFrame()
    {
        ImGuiNET.ImGui.Render();
    }

    public void RenderDrawData(object drawData)
    {
        if (drawData is not ImDrawDataPtr imguiDrawData)
            throw new ArgumentException("drawData must be ImDrawDataPtr", nameof(drawData));

        SetPerFrameData();

        if (imguiDrawData.CmdListsCount == 0)
            return;

        if (m_vertexBufferSize < imguiDrawData.TotalVtxCount)
        {
            m_vertexBuffer.Dispose();
            m_vertexBufferSize = imguiDrawData.TotalVtxCount + 5000;
            m_vertexBuffer = new DynamicVertexBuffer(graphicsDevice, m_vertexDeclaration, m_vertexBufferSize, BufferUsage.None);
        }

        if (m_indexBufferSize < imguiDrawData.TotalIdxCount)
        {
            m_indexBuffer.Dispose();
            m_indexBufferSize = imguiDrawData.TotalIdxCount + 10000;
            m_indexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, m_indexBufferSize, BufferUsage.None);
        }

        int vtxOffset = 0;
        int idxOffset = 0;

        var vertexData = new VertexPositionColorTexture[imguiDrawData.TotalVtxCount];
        var indexData = new ushort[imguiDrawData.TotalIdxCount];

        for (int n = 0; n < imguiDrawData.CmdListsCount; n++)
        {
            var cmdList = imguiDrawData.CmdLists[n];

            for (int i = 0; i < cmdList.VtxBuffer.Size; i++)
            {
                var v = cmdList.VtxBuffer[i];
                vertexData[vtxOffset + i] = new VertexPositionColorTexture(
                    new Vector3(v.pos.X, v.pos.Y, 0f),
                    new Color(
                        (v.col & 0xFF),
                        (v.col >> 8) & 0xFF,
                        (v.col >> 16) & 0xFF,
                        (v.col >> 24) & 0xFF),
                    new Vector2(v.uv.X, v.uv.Y));
            }

            for (int i = 0; i < cmdList.IdxBuffer.Size; i++)
            {
                indexData[idxOffset + i] = cmdList.IdxBuffer[i];
            }

            vtxOffset += cmdList.VtxBuffer.Size;
            idxOffset += cmdList.IdxBuffer.Size;
        }

        m_vertexBuffer.SetData(vertexData, 0, imguiDrawData.TotalVtxCount);
        m_indexBuffer.SetData(indexData, 0, imguiDrawData.TotalIdxCount);

        var gd = graphicsDevice;

        gd.SetVertexBuffer(m_vertexBuffer);
        gd.Indices = m_indexBuffer;

        gd.BlendState = m_blendState;
        gd.RasterizerState = m_rasterizerState;
        gd.DepthStencilState = m_depthStencilState;

        gd.Viewport = graphicsDevice.Viewport;

        m_effect.CurrentTechnique.Passes[0].Apply();

        int offset = 0;

        for (int n = 0; n < imguiDrawData.CmdListsCount; n++)
        {
            var cmdList = imguiDrawData.CmdLists[n];

            for (int cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
            {
                var pcmd = cmdList.CmdBuffer[cmdI];

                gd.ScissorRectangle = new Rectangle(
                    (int)pcmd.ClipRect.X,
                    (int)pcmd.ClipRect.Y,
                    (int)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                    (int)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                Texture2D? tex = GetTextureById(pcmd.TextureId);

                if (tex == null)
                {
                    Texture2D whiteTexture = new Texture2D(graphicsDevice, 1, 1);
                    whiteTexture.SetData(new[] { Color.White });
                    tex = whiteTexture;
                }

                m_effect.Texture = tex;
                m_effect.CurrentTechnique.Passes[0].Apply();

                gd.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    offset,
                    (int)pcmd.ElemCount / 3);

                offset += (int)pcmd.ElemCount;
            }
        }

        gd.ScissorRectangle = new Rectangle(0, 0, gd.Viewport.Width, gd.Viewport.Height);
    }

    public IntPtr BindTexture(ITexture2D texture)
    {
        if (texture is not Texture2D mgTexture)
            throw new ArgumentException("texture is not a MonoGame Texture2D", nameof(texture));
        
        return BindTexture(mgTexture);
    }

    private IntPtr BindTexture(Texture2D mgTexture)
    {
        if (mgTexture == null)
            throw new ArgumentNullException(nameof(mgTexture));

        foreach (var kvp in m_loadedTextures)
        {
            if (kvp.Value == mgTexture)
                return kvp.Key;
        }

        var id = m_lastTextureId;
        m_loadedTextures[id] = mgTexture;
        m_lastTextureId = new IntPtr(m_lastTextureId.ToInt64() + 1);
        return id;
    }

    private Texture2D? GetTextureById(IntPtr id)
    {
        if (id == IntPtr.Zero)
            return null;
        return m_loadedTextures.GetValueOrDefault(id);
    }

    private void SetPerFrameData()
    {
        var vp = graphicsDevice.Viewport;
        m_projectionMatrix = Matrix.CreateOrthographicOffCenter(0f, vp.Width, vp.Height, 0f, 0f, 1f);
        if (m_effect is { } basicEffect)
        {
            basicEffect.Projection = m_projectionMatrix;
        }
    }

    public void Dispose()
    {
        m_vertexBuffer?.Dispose();
        m_indexBuffer?.Dispose();
        m_effect?.Dispose();
        m_rasterizerState?.Dispose();
        m_blendState?.Dispose();
        m_depthStencilState?.Dispose();
    }
}