using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Veldrid;

namespace InnoInternal.ImGui.Bridge;

/// <summary>
/// ImGui.NET Bridge Renderer for Veldrid.
/// This is modified by the original ImGuiRenderer from Veldrid.ImGui.
/// </summary>
internal class ImGuiNETVeldridBridge : IDisposable
{
    private readonly GraphicsDevice m_graphicsDevice;
    private readonly Assembly m_assembly;
    private readonly ImGuiNETColorSpaceHandling m_colorSpaceHandling;

    // Device objects
    private DeviceBuffer? m_vertexBuffer;
    private DeviceBuffer? m_indexBuffer;
    private DeviceBuffer? m_projMatrixBuffer;
    private Texture? m_fontTexture;
    private Shader? m_vertexShader;
    private Shader? m_fragmentShader;
    private ResourceLayout? m_layout;
    private ResourceLayout? m_textureLayout;
    private Pipeline? m_pipeline;
    private ResourceSet? m_mainResourceSet;
    private ResourceSet? m_fontTextureResourceSet;
    private readonly IntPtr m_fontAtlasId = 1;

    private int m_windowWidth;
    private int m_windowHeight;
    private readonly Vector2 m_scaleFactor = Vector2.One;

    // Image trackers
    private readonly Dictionary<TextureView, ResourceSetInfo> m_setsByView = new();
    private readonly Dictionary<Texture, TextureView> m_autoViewsByTexture = new();
    private readonly Dictionary<IntPtr, ResourceSetInfo> m_viewsById = new();
    private readonly List<IDisposable> m_ownedResources = new();
    private int m_lastAssignedId = 100;
    private bool m_frameBegun;
    
    
    // ============================================================
    // Initialization and Loads
    // ============================================================
    
    #region Inits

    /// <summary>
    /// Constructs a new ImGuiRenderer.
    /// </summary>
    /// <param name="gd">The GraphicsDevice used to create and update resources.</param>
    /// <param name="outputDescription">The output format.</param>
    /// <param name="width">The initial width of the rendering target. Can be resized.</param>
    /// <param name="height">The initial height of the rendering target. Can be resized.</param>
    /// <param name="colorSpaceHandling">Identifies how the renderer should treat vertex colors.</param>
    public ImGuiNETVeldridBridge(GraphicsDevice gd, OutputDescription outputDescription, int width, int height, ImGuiNETColorSpaceHandling colorSpaceHandling)
    {
        m_graphicsDevice = gd;
        m_assembly = typeof(ImGuiNETVeldridBridge).GetTypeInfo().Assembly;
        m_colorSpaceHandling = colorSpaceHandling;
        m_windowWidth = width;
        m_windowHeight = height;

        IntPtr context = ImGuiNET.ImGui.CreateContext();
        ImGuiNET.ImGui.SetCurrentContext(context);

        ImGuiNET.ImGui.GetIO().Fonts.AddFontDefault();
        ImGuiNET.ImGui.GetIO().Fonts.Flags |= ImFontAtlasFlags.NoBakedLines;

        CreateDeviceResources(outputDescription);

        SetPerFrameImGuiData(1f / 60f);
    }
    
    private void CreateDeviceResources(OutputDescription outputDescription)
    {
        ResourceFactory factory = m_graphicsDevice.ResourceFactory;
        m_vertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
        m_vertexBuffer.Name = "ImGui.NET Vertex Buffer";
        m_indexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
        m_indexBuffer.Name = "ImGui.NET Index Buffer";

        m_projMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        m_projMatrixBuffer.Name = "ImGui.NET Projection Buffer";

        byte[] vertexShaderBytes = LoadEmbeddedShaderCode(m_graphicsDevice.ResourceFactory, "imgui-vertex", ShaderStages.Vertex, m_colorSpaceHandling);
        byte[] fragmentShaderBytes = LoadEmbeddedShaderCode(m_graphicsDevice.ResourceFactory, "imgui-frag", ShaderStages.Fragment, m_colorSpaceHandling);
        m_vertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, m_graphicsDevice.BackendType == GraphicsBackend.Vulkan ? "main" : "VS"));
        m_vertexShader.Name = "ImGui.NET Vertex Shader";
        m_fragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, m_graphicsDevice.BackendType == GraphicsBackend.Vulkan ? "main" : "FS"));
        m_fragmentShader.Name = "ImGui.NET Fragment Shader";

        VertexLayoutDescription[] vertexLayouts =
        [
            new(
                new VertexElementDescription("in_position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("in_color", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm))
        ];

        m_layout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("ProjectionMatrixBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("MainSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
        m_layout.Name = "ImGui.NET Resource Layout";
        m_textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)));
        m_textureLayout.Name = "ImGui.NET Texture Layout";

        GraphicsPipelineDescription pd = new GraphicsPipelineDescription(
            BlendStateDescription.SingleAlphaBlend,
            new DepthStencilStateDescription(false, false, ComparisonKind.Always),
            new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, true, true),
            PrimitiveTopology.TriangleList,
            new ShaderSetDescription(
                vertexLayouts,
                [m_vertexShader, m_fragmentShader],
                [
                    new SpecializationConstant(0, m_graphicsDevice.IsClipSpaceYInverted),
                    new SpecializationConstant(1, m_colorSpaceHandling == ImGuiNETColorSpaceHandling.Legacy)
                ]),
            [m_layout, m_textureLayout],
            outputDescription,
            ResourceBindingModel.Default);
        m_pipeline = factory.CreateGraphicsPipeline(ref pd);
        m_pipeline.Name = "ImGui.NET Pipeline";

        m_mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(m_layout,
            m_projMatrixBuffer,
            m_graphicsDevice.PointSampler));
        m_mainResourceSet.Name = "ImGui.NET Main Resource Set";

        RecreateFontDeviceTexture();
    }
    
        private byte[] LoadEmbeddedShaderCode(
        ResourceFactory factory,
        string name,
        ShaderStages stage,
        ImGuiNETColorSpaceHandling colorSpaceHandling)
    {
        switch (factory.BackendType)
        {
            case GraphicsBackend.Direct3D11:
            {
                if (stage == ShaderStages.Vertex && colorSpaceHandling == ImGuiNETColorSpaceHandling.Legacy) { name += "-legacy"; }
                string resourceName = name + ".hlsl.bytes";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.OpenGL:
            {
                if (stage == ShaderStages.Vertex && colorSpaceHandling == ImGuiNETColorSpaceHandling.Legacy) { name += "-legacy"; }
                string resourceName = name + ".glsl";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.OpenGLES:
            {
                if (stage == ShaderStages.Vertex && colorSpaceHandling == ImGuiNETColorSpaceHandling.Legacy) { name += "-legacy"; }
                string resourceName = name + ".glsles";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.Vulkan:
            {
                string resourceName = name + ".spv";
                return GetEmbeddedResourceBytes(resourceName);
            }
            case GraphicsBackend.Metal:
            {
                string resourceName = name + ".metallib";
                return GetEmbeddedResourceBytes(resourceName);
            }
            default:
                throw new NotImplementedException();
        }
    }

    private byte[] GetEmbeddedResourceBytes(string shortName)
    {
        var resources = m_assembly.GetManifestResourceNames();
        var match = resources.FirstOrDefault(r => r.EndsWith(shortName, StringComparison.OrdinalIgnoreCase));
        if (match == null) throw new FileNotFoundException($"Embedded resource '{shortName}' not found.");

        using Stream s = m_assembly.GetManifestResourceStream(match)!;
        byte[] ret = new byte[s.Length];
        s.ReadExactly(ret, 0, (int)s.Length);
        return ret;
    }
    
    #endregion
    
    // ============================================================
    // Texture Bindings
    // ============================================================
    
    #region Texture Bindings

    /// <summary>
    /// Gets or creates a handle for a texture to be drawn with ImGui.
    /// Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, TextureView textureView)
    {
        if (m_setsByView.TryGetValue(textureView, out ResourceSetInfo rsi)) return rsi.imGuiBinding;
        ResourceSet resourceSet = factory.CreateResourceSet(new ResourceSetDescription(m_textureLayout, textureView));
        resourceSet.Name = $"ImGui.NET {textureView.Name} Resource Set";

        m_lastAssignedId++;
        rsi = new ResourceSetInfo(m_lastAssignedId, resourceSet);

        m_setsByView.Add(textureView, rsi);
        m_viewsById.Add(rsi.imGuiBinding, rsi);
        m_ownedResources.Add(resourceSet);

        return rsi.imGuiBinding;
    }

    /// <summary>
    /// Removes the ImGui binding for the given texture view.
    /// </summary>
    public void RemoveImGuiBinding(TextureView textureView)
    {
        if (m_setsByView.Remove(textureView, out ResourceSetInfo rsi))
        {
            m_viewsById.Remove(rsi.imGuiBinding);
            m_ownedResources.Remove(rsi.resourceSet);
            rsi.resourceSet.Dispose();
        }
    }

    /// <summary>
    /// Gets or creates a handle for a texture to be drawn with ImGui.
    /// Pass the returned handle to Image() or ImageButton().
    /// </summary>
    public IntPtr GetOrCreateImGuiBinding(ResourceFactory factory, Texture texture)
    {
        if (!m_autoViewsByTexture.TryGetValue(texture, out var textureView))
        {
            textureView = factory.CreateTextureView(texture);
            textureView.Name = $"ImGui.NET {texture.Name} View";
            m_autoViewsByTexture.Add(texture, textureView);
            m_ownedResources.Add(textureView);
        }

        return GetOrCreateImGuiBinding(factory, textureView);
    }

    /// <summary>
    /// Removes the ImGui binding for the given texture.
    /// </summary>
    public void RemoveImGuiBinding(Texture texture)
    {
        if (m_autoViewsByTexture.Remove(texture, out var textureView))
        {
            m_ownedResources.Remove(textureView);
            textureView.Dispose();
            RemoveImGuiBinding(textureView);
        }
    }

    /// <summary>
    /// Retrieves the shader texture binding for the given helper handle.
    /// </summary>
    public ResourceSet GetImageResourceSet(IntPtr imGuiBinding)
    {
        if (!m_viewsById.TryGetValue(imGuiBinding, out ResourceSetInfo rsi))
        {
            throw new InvalidOperationException("No registered ImGui binding with id " + imGuiBinding.ToString());
        }

        return rsi.resourceSet;
    }

    /// <summary>
    /// Clears all cached image resources.
    /// </summary>
    public void ClearCachedImageResources()
    {
        foreach (IDisposable resource in m_ownedResources)
        {
            resource.Dispose();
        }

        m_ownedResources.Clear();
        m_setsByView.Clear();
        m_viewsById.Clear();
        m_autoViewsByTexture.Clear();
        m_lastAssignedId = 100;
    }
    
    #endregion
    
    // ============================================================
    // Fonts
    // ============================================================
    
    #region Fonts
    
    /// <summary>
    /// Recreates the device texture used to render text.
    /// </summary>
    public unsafe void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
        // Build
        io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out int width, out int height, out int bytesPerPixel);

        // Store our identifier
        io.Fonts.SetTexID(m_fontAtlasId);

        m_fontTexture?.Dispose();
        m_fontTexture = m_graphicsDevice.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
            (uint)width,
            (uint)height,
            1,
            1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled));
        m_fontTexture.Name = "ImGui.NET Font Texture";
        m_graphicsDevice.UpdateTexture(
            m_fontTexture,
            (IntPtr)pixels,
            (uint)(bytesPerPixel * width * height),
            0,
            0,
            0,
            (uint)width,
            (uint)height,
            1,
            0,
            0);

        m_fontTextureResourceSet?.Dispose();
        m_fontTextureResourceSet = m_graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(m_textureLayout, m_fontTexture));
        m_fontTextureResourceSet.Name = "ImGui.NET Font Texture Resource Set";

        io.Fonts.ClearTexData();
    }
    
    #endregion
    
    // ============================================================
    // Lifecycle
    // ============================================================

    #region Lifecycle
    
    /// <summary>
    /// Renders the ImGui draw list data.
    /// </summary>
    public void Render(GraphicsDevice gd, CommandList cl)
    {
        if (m_frameBegun)
        {
            m_frameBegun = false;
            ImGuiNET.ImGui.Render();
            RenderImDrawData(ImGuiNET.ImGui.GetDrawData(), gd, cl);
        }
    }
    
        private unsafe void RenderImDrawData(ImDrawDataPtr drawData, GraphicsDevice gd, CommandList cl)
    {
        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (drawData.CmdListsCount == 0)
        {
            return;
        }

        uint totalVbSize = (uint)(drawData.TotalVtxCount * sizeof(ImDrawVert));
        if (m_vertexBuffer != null && totalVbSize > m_vertexBuffer.SizeInBytes)
        {
            m_vertexBuffer.Dispose();
            m_vertexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalVbSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            m_vertexBuffer.Name = $"ImGui.NET Vertex Buffer";
        }

        uint totalIbSize = (uint)(drawData.TotalIdxCount * sizeof(ushort));
        if (m_indexBuffer != null && totalIbSize > m_indexBuffer.SizeInBytes)
        {
            m_indexBuffer.Dispose();
            m_indexBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalIbSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            m_indexBuffer.Name = $"ImGui.NET Index Buffer";
        }

        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];

            cl.UpdateBuffer(
                m_vertexBuffer,
                vertexOffsetInVertices * (uint)sizeof(ImDrawVert),
                cmdList.VtxBuffer.Data,
                (uint)(cmdList.VtxBuffer.Size * sizeof(ImDrawVert)));

            cl.UpdateBuffer(
                m_indexBuffer,
                indexOffsetInElements * sizeof(ushort),
                cmdList.IdxBuffer.Data,
                (uint)(cmdList.IdxBuffer.Size * sizeof(ushort)));

            vertexOffsetInVertices += (uint)cmdList.VtxBuffer.Size;
            indexOffsetInElements += (uint)cmdList.IdxBuffer.Size;
        }

        // Setup orthographic projection matrix into our constant buffer
        {
            var io = ImGuiNET.ImGui.GetIO();

            Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
                0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            m_graphicsDevice.UpdateBuffer(m_projMatrixBuffer, 0, ref mvp);
        }

        cl.SetVertexBuffer(0, m_vertexBuffer);
        cl.SetIndexBuffer(m_indexBuffer, IndexFormat.UInt16);
        cl.SetPipeline(m_pipeline);
        cl.SetGraphicsResourceSet(0, m_mainResourceSet);

        drawData.ScaleClipRects(ImGuiNET.ImGui.GetIO().DisplayFramebufferScale);

        // Render command lists
        int vtxOffset = 0;
        int idxOffset = 0;
        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];
            for (int cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
            {
                ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdI];
                if (pcmd.UserCallback != IntPtr.Zero)
                {
                    throw new NotImplementedException();
                }

                if (pcmd.TextureId != IntPtr.Zero)
                {
                    cl.SetGraphicsResourceSet(1,
                        pcmd.TextureId == m_fontAtlasId
                            ? m_fontTextureResourceSet
                            : GetImageResourceSet(pcmd.TextureId));
                }

                cl.SetScissorRect(
                    0,
                    (uint)pcmd.ClipRect.X,
                    (uint)pcmd.ClipRect.Y,
                    (uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                    (uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                cl.DrawIndexed(pcmd.ElemCount, 1, pcmd.IdxOffset + (uint)idxOffset, (int)(pcmd.VtxOffset + vtxOffset), 0);
            }

            idxOffset += cmdList.IdxBuffer.Size;
            vtxOffset += cmdList.VtxBuffer.Size;
        }
    }


    /// <summary>
    /// Updates ImGui input and IO configuration state.
    /// </summary>
    public void Update(float deltaSeconds, InputSnapshot snapshot)
    {
        if (m_frameBegun)
        {
            ImGuiNET.ImGui.Render();
        }
        
        SetPerFrameImGuiData(deltaSeconds);
        
        UpdateImGuiInput(snapshot);
        
        m_frameBegun = true;
        ImGuiNET.ImGui.NewFrame();
    }
    
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
        io.DisplaySize = new Vector2(
            m_windowWidth / m_scaleFactor.X,
            m_windowHeight / m_scaleFactor.Y);
        io.DisplayFramebufferScale = m_scaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }
    
    #endregion
    
    // ============================================================
    // Events
    // ============================================================
    
    #region Events
    
    /// <summary>
    /// Resize the renderer's output window.
    /// </summary>
    public void WindowResized(int width, int height)
    {
        m_windowWidth = width;
        m_windowHeight = height;
    }
    
    private bool TryMapKey(Key key, out ImGuiKey result)
    {
        ImGuiKey KeyToImGuiKeyShortcut(Key keyToConvert, Key startKey1, ImGuiKey startKey2)
        {
            int changeFromStart1 = (int)keyToConvert - (int)startKey1;
            return startKey2 + changeFromStart1;
        }

        switch (key)
        {
            case >= Key.F1 and <= Key.F12:
                result = KeyToImGuiKeyShortcut(key, Key.F1, ImGuiKey.F1);
                return true;
            case >= Key.Keypad0 and <= Key.Keypad9:
                result = KeyToImGuiKeyShortcut(key, Key.Keypad0, ImGuiKey.Keypad0);
                return true;
            case >= Key.A and <= Key.Z:
                result = KeyToImGuiKeyShortcut(key, Key.A, ImGuiKey.A);
                return true;
            case >= Key.Number0 and <= Key.Number9:
                result = KeyToImGuiKeyShortcut(key, Key.Number0, ImGuiKey._0);
                return true;
            default:
                switch (key)
                {
                    case Key.ShiftLeft:
                    case Key.ShiftRight:
                        result = ImGuiKey.ModShift;
                        return true;
                    case Key.ControlLeft:
                    case Key.ControlRight:
                        result = ImGuiKey.ModCtrl;
                        return true;
                    case Key.AltLeft:
                    case Key.AltRight:
                        result = ImGuiKey.ModAlt;
                        return true;
                    case Key.WinLeft:
                    case Key.WinRight:
                        result = ImGuiKey.ModSuper;
                        return true;
                    case Key.Menu:
                        result = ImGuiKey.Menu;
                        return true;
                    case Key.Up:
                        result = ImGuiKey.UpArrow;
                        return true;
                    case Key.Down:
                        result = ImGuiKey.DownArrow;
                        return true;
                    case Key.Left:
                        result = ImGuiKey.LeftArrow;
                        return true;
                    case Key.Right:
                        result = ImGuiKey.RightArrow;
                        return true;
                    case Key.Enter:
                        result = ImGuiKey.Enter;
                        return true;
                    case Key.Escape:
                        result = ImGuiKey.Escape;
                        return true;
                    case Key.Space:
                        result = ImGuiKey.Space;
                        return true;
                    case Key.Tab:
                        result = ImGuiKey.Tab;
                        return true;
                    case Key.BackSpace:
                        result = ImGuiKey.Backspace;
                        return true;
                    case Key.Insert:
                        result = ImGuiKey.Insert;
                        return true;
                    case Key.Delete:
                        result = ImGuiKey.Delete;
                        return true;
                    case Key.PageUp:
                        result = ImGuiKey.PageUp;
                        return true;
                    case Key.PageDown:
                        result = ImGuiKey.PageDown;
                        return true;
                    case Key.Home:
                        result = ImGuiKey.Home;
                        return true;
                    case Key.End:
                        result = ImGuiKey.End;
                        return true;
                    case Key.CapsLock:
                        result = ImGuiKey.CapsLock;
                        return true;
                    case Key.ScrollLock:
                        result = ImGuiKey.ScrollLock;
                        return true;
                    case Key.PrintScreen:
                        result = ImGuiKey.PrintScreen;
                        return true;
                    case Key.Pause:
                        result = ImGuiKey.Pause;
                        return true;
                    case Key.NumLock:
                        result = ImGuiKey.NumLock;
                        return true;
                    case Key.KeypadDivide:
                        result = ImGuiKey.KeypadDivide;
                        return true;
                    case Key.KeypadMultiply:
                        result = ImGuiKey.KeypadMultiply;
                        return true;
                    case Key.KeypadSubtract:
                        result = ImGuiKey.KeypadSubtract;
                        return true;
                    case Key.KeypadAdd:
                        result = ImGuiKey.KeypadAdd;
                        return true;
                    case Key.KeypadDecimal:
                        result = ImGuiKey.KeypadDecimal;
                        return true;
                    case Key.KeypadEnter:
                        result = ImGuiKey.KeypadEnter;
                        return true;
                    case Key.Tilde:
                        result = ImGuiKey.GraveAccent;
                        return true;
                    case Key.Minus:
                        result = ImGuiKey.Minus;
                        return true;
                    case Key.Plus:
                        result = ImGuiKey.Equal;
                        return true;
                    case Key.BracketLeft:
                        result = ImGuiKey.LeftBracket;
                        return true;
                    case Key.BracketRight:
                        result = ImGuiKey.RightBracket;
                        return true;
                    case Key.Semicolon:
                        result = ImGuiKey.Semicolon;
                        return true;
                    case Key.Quote:
                        result = ImGuiKey.Apostrophe;
                        return true;
                    case Key.Comma:
                        result = ImGuiKey.Comma;
                        return true;
                    case Key.Period:
                        result = ImGuiKey.Period;
                        return true;
                    case Key.Slash:
                        result = ImGuiKey.Slash;
                        return true;
                    case Key.BackSlash:
                    case Key.NonUSBackSlash:
                        result = ImGuiKey.Backslash;
                        return true;
                    default:
                        result = ImGuiKey.GamepadBack;
                        return false;
                }
        }
    }

    private void UpdateImGuiInput(InputSnapshot snapshot)
    {
        ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
        io.AddMousePosEvent(snapshot.MousePosition.X, snapshot.MousePosition.Y);
        io.AddMouseButtonEvent(0, snapshot.IsMouseDown(MouseButton.Left));
        io.AddMouseButtonEvent(1, snapshot.IsMouseDown(MouseButton.Right));
        io.AddMouseButtonEvent(2, snapshot.IsMouseDown(MouseButton.Middle));
        io.AddMouseButtonEvent(3, snapshot.IsMouseDown(MouseButton.Button1));
        io.AddMouseButtonEvent(4, snapshot.IsMouseDown(MouseButton.Button2));
        io.AddMouseWheelEvent(0f, snapshot.WheelDelta);

        foreach (var t in snapshot.KeyCharPresses)
        {
            io.AddInputCharacter(t);
        }

        foreach (var keyEvent in snapshot.KeyEvents)
        {
            if (TryMapKey(keyEvent.Key, out ImGuiKey imguikey))
            {
                io.AddKeyEvent(imguikey, keyEvent.Down);
            }
        }
    }
    
    /// <summary>
    /// Frees all graphics resources used by the renderer.
    /// </summary>
    public void Dispose()
    {
        m_vertexBuffer?.Dispose();
        m_indexBuffer?.Dispose();
        m_projMatrixBuffer?.Dispose();
        m_fontTexture?.Dispose();
        m_vertexShader?.Dispose();
        m_fragmentShader?.Dispose();
        m_layout?.Dispose();
        m_textureLayout?.Dispose();
        m_pipeline?.Dispose();
        m_mainResourceSet?.Dispose();
        m_fontTextureResourceSet?.Dispose();

        foreach (IDisposable resource in m_ownedResources)
        {
            resource.Dispose();
        }
    }
    
    #endregion
    
    // ============================================================
    // Structs
    // ============================================================

    #region Structs
    
    private struct ResourceSetInfo(IntPtr imGuiBinding, ResourceSet resourceSet)
    {
        public readonly IntPtr imGuiBinding = imGuiBinding;
        public readonly ResourceSet resourceSet = resourceSet;
    }
    
    #endregion
}