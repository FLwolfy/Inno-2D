// NOTE: This file is referenced from ImGuiNET.MonoGame package and with slight modification.
//       To see more details from the original package, visit here:
//       https://github.com/tsMezotic/MonoGame.ImGuiNet

using System.Net.Mime;
using System.Runtime.InteropServices;

using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Bridge;
using InnoInternal.Resource.Bridge;
using InnoInternal.Resource.Impl;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ImGuiNET;

namespace InnoInternal.ImGui.Bridge;

internal class ImGuiNETMonoGameRenderer : IImGuiRenderer
{
    // Graphics
    private GraphicsDevice graphicsDevice => MonoGameRenderAPI.graphicsDevice;
    private Game m_game = null!;
    
    private BasicEffect m_effect = null!;
    private RasterizerState m_rasterizerState = null!;
    
    private byte[] m_vertexData = null!;
    private VertexBuffer m_vertexBuffer = null!;
    private int m_vertexBufferSize;

    private byte[] m_indexData = null!;
    private IndexBuffer m_indexBuffer = null!;
    private int m_indexBufferSize;

    private IImGuiContext m_imGuiContext = null!;
    private IntPtr m_imGuiMainContextPtr;
    private IntPtr m_imGuiVirtualContextPtr;
    
    // Properties
    public IImGuiContext context => m_imGuiContext;
    public IntPtr mainMainContextPtr => m_imGuiMainContextPtr;
    public IntPtr virtualContextPtr => m_imGuiVirtualContextPtr;

    // Textures
    private readonly Dictionary<IntPtr, Texture2D> m_loadedTextures = new();
    
    private int m_textureId;
    private IntPtr? m_fontTextureId;

    // Input
    private int m_scrollWheelValue;
    private int m_horizontalScrollWheelValue;
    private readonly float m_wheelDelta = 120;
    private readonly Keys[] m_allKeys = Enum.GetValues<Keys>();

    public void Initialize(object windowHolder)
    {
        if (windowHolder == null || windowHolder is not Game game)
            throw new ArgumentException("graphicsHolder is null or not Game object", nameof(windowHolder));
        
        m_game = game;
        m_imGuiContext = new ImGuiNETContext(this);
        m_rasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            DepthBias = 0,
            FillMode = FillMode.Solid,
            MultiSampleAntiAlias = false,
            ScissorTestEnable = true,
            SlopeScaleDepthBias = 0
        };
        
        // Virtual Context
        m_imGuiVirtualContextPtr = ImGuiNET.ImGui.CreateContext();
        ImGuiNET.ImGui.SetCurrentContext(m_imGuiVirtualContextPtr);
        RebuildFontAtlas();
        
        // Main Context
        m_imGuiMainContextPtr = ImGuiNET.ImGui.CreateContext();
        ImGuiNET.ImGui.SetCurrentContext(m_imGuiMainContextPtr);
        SetupInput();
        SetupFlags();
        SetupThemes();
        RebuildFontAtlas();
    }

    #region ImGuiRenderer

    /// <summary>
    /// Creates a texture and loads the font data from ImGui. Should be called when the <see cref="GraphicsDevice" /> is initialized but before any rendering is done
    /// </summary>
    private unsafe void RebuildFontAtlas()
    {
        // Get font texture from ImGui
        var io = ImGuiNET.ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

        // Copy the data to a managed array
        var pixels = new byte[width * height * bytesPerPixel];
        unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

        // Create and register the texture as an XNA texture
        var tex2d = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);
        tex2d.SetData(pixels);

        // Should a texture already have been build previously, unbind it first so it can be deallocated
        if (m_fontTextureId.HasValue) UnbindTexture(m_fontTextureId.Value);

        // Bind the new texture to an ImGui-friendly id
        m_fontTextureId = BindTexture(tex2d);

        // Let ImGui know where to find the texture
        io.Fonts.SetTexID(m_fontTextureId.Value);
        io.Fonts.ClearTexData(); // Clears CPU side texture data
    }

    /// <summary>
    /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="MediaTypeNames.Image" />. That pointer is then used by ImGui to let us know what texture to draw
    /// </summary>
    public IntPtr BindTexture(ITexture2D texture)
    {
        if (texture == null || texture is not MonoGameTexture2D mgTexture)
            throw new ArgumentException("texture is null or not MonoGameTexture2D", nameof(texture));
        
        return BindTexture(mgTexture.rawTexture);
    }
    
    private IntPtr BindTexture(Texture2D mgTexture)
    {
        var id = new IntPtr(m_textureId++);
        
        m_loadedTextures.Add(id, mgTexture);

        return id;
    }

    /// <summary>
    /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
    /// </summary>
    private void UnbindTexture(IntPtr textureId)
    {
        m_loadedTextures.Remove(textureId);
    }

    /// <summary>
    /// Sets up ImGui for a new frame, should be called at frame start
    /// </summary>
    /// <param name="deltaTime">The deltaTime between two render frame in seconds</param>
    public virtual void BeginLayout(float deltaTime)
    {
        // Virtual Context
        ImGuiNET.ImGui.SetCurrentContext(m_imGuiVirtualContextPtr);
        ImGuiNET.ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
        ImGuiNET.ImGui.NewFrame();
        
        // Main Context
        ImGuiNET.ImGui.SetCurrentContext(m_imGuiMainContextPtr);
        ImGuiNET.ImGui.GetIO().DeltaTime = deltaTime;
        UpdateInput();
        ImGuiNET.ImGui.NewFrame();
        ImGuiNET.ImGui.DockSpaceOverViewport(ImGuiNET.ImGui.GetMainViewport().ID);
    }

    /// <summary>
    /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
    /// </summary>
    public virtual void EndLayout()
    {
        // Virtual Context
        ImGuiNET.ImGui.SetCurrentContext(m_imGuiVirtualContextPtr);
        ImGuiNET.ImGui.EndFrame();
        
        // Main Context
        ImGuiNET.ImGui.SetCurrentContext(m_imGuiMainContextPtr);
        ImGuiNET.ImGui.Render();
        { RenderDrawData(ImGuiNET.ImGui.GetDrawData()); }
        
        // Set Cursor
        var imGuiCursor = ImGuiNET.ImGui.GetMouseCursor();
        MouseCursor mgCursor = imGuiCursor switch
        {
            ImGuiMouseCursor.Arrow => MouseCursor.Arrow,
            ImGuiMouseCursor.Hand => MouseCursor.Hand,
            ImGuiMouseCursor.TextInput => MouseCursor.IBeam,
            ImGuiMouseCursor.ResizeAll => MouseCursor.SizeAll,
            ImGuiMouseCursor.ResizeNS => MouseCursor.SizeNS,
            ImGuiMouseCursor.ResizeEW => MouseCursor.SizeWE,
            ImGuiMouseCursor.ResizeNESW => MouseCursor.SizeNESW,
            ImGuiMouseCursor.ResizeNWSE => MouseCursor.SizeNWSE,
            _ => MouseCursor.Arrow,
        };
        Mouse.SetCursor(mgCursor);
    }

    #endregion ImGuiRenderer

    #region Setup & Update

    private void SetupFlags()
    {
        var io = ImGuiNET.ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        
        // Not Supported with MonoGame
        // io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable; 
    }

    /// <summary>
    /// Setup key input event handler.
    /// </summary>
    private void SetupInput()
    {
        var io = ImGuiNET.ImGui.GetIO();

        // MonoGame-specific //////////////////////
        m_game.Window.TextInput += (s, a) =>
        {
            if (a.Character == '\t') return;
            io.AddInputCharacter(a.Character);
        };

        ///////////////////////////////////////////

        // FNA-specific ///////////////////////////
        //TextInputEXT.TextInput += c =>
        //{
        //    if (c == '\t') return;

        //    ImGui.GetIO().AddInputCharacter(c);
        //};
        ///////////////////////////////////////////
    }

    private void SetupThemes()
    {
        ImGuiNET.ImGui.StyleColorsDark();
        
        var style = ImGuiNET.ImGui.GetStyle();
        var colors = style.Colors;

        // Window Background
        colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.10f, 0.10f, 0.11f, 1.0f);

        // Headers
        colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.20f, 0.205f, 0.25f, 1.0f);
        colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.35f, 0.30f, 0.45f, 1.0f); // muted purple
        colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.30f, 0.25f, 0.40f, 1.0f);

        // Buttons
        colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.20f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.35f, 0.30f, 0.45f, 1.0f);
        colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.30f, 0.25f, 0.40f, 1.0f);

        // Frame BG
        colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.18f, 0.18f, 0.20f, 1.0f);
        colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.35f, 0.30f, 0.45f, 1.0f);
        colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.30f, 0.25f, 0.40f, 1.0f);

        // Tabs
        colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.13f, 0.13f, 0.16f, 1.0f);
        colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.45f, 0.35f, 0.60f, 1.0f);
        colors[(int)ImGuiCol.TabSelected] = new System.Numerics.Vector4(0.38f, 0.32f, 0.50f, 1.0f);
        colors[(int)ImGuiCol.TabDimmed] = new System.Numerics.Vector4(0.10f, 0.10f, 0.12f, 1.0f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new System.Numerics.Vector4(0.28f, 0.23f, 0.36f, 1.0f);

        // Title bar
        colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.12f, 0.12f, 0.15f, 1.0f);
        colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.18f, 0.18f, 0.22f, 1.0f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.08f, 0.08f, 0.10f, 1.0f);

        // Optional: Resize grip, scrollbar, etc. for more polish
        colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.3f, 0.3f, 0.35f, 0.6f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.4f, 0.35f, 0.5f, 0.7f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.30f, 0.30f, 0.35f, 1.0f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.40f, 0.35f, 0.50f, 1.0f);
        colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.7f, 0.6f, 0.9f, 1.0f);

        // Style tweaks
        style.WindowRounding = 4.0f;
        style.FrameRounding = 3.0f;
        style.ScrollbarRounding = 3.0f;
        style.GrabRounding = 3.0f;
    }
    

    /// <summary>
    /// Updates the <see cref="Effect" /> to the current matrices and texture
    /// </summary>
    private Effect UpdateEffect(Texture2D texture)
    {
        m_effect = m_effect ?? new BasicEffect(graphicsDevice);

        var io = ImGuiNET.ImGui.GetIO();

        m_effect.World = Matrix.Identity;
        m_effect.View = Matrix.Identity;
        m_effect.Projection = Matrix.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
        m_effect.TextureEnabled = true;
        m_effect.Texture = texture;
        m_effect.VertexColorEnabled = true;

        return m_effect;
    }

    /// <summary>
    /// Sends XNA input state to ImGui
    /// </summary>
    private void UpdateInput()
    {
        if (!m_game.IsActive) return;
            
        var io = ImGuiNET.ImGui.GetIO();

        var mouse = Mouse.GetState();
        var keyboard = Keyboard.GetState();

        io.AddMousePosEvent(mouse.X, mouse.Y);
        io.AddMouseButtonEvent(0, mouse.LeftButton == ButtonState.Pressed);
        io.AddMouseButtonEvent(1, mouse.RightButton == ButtonState.Pressed);
        io.AddMouseButtonEvent(2, mouse.MiddleButton == ButtonState.Pressed);
        io.AddMouseButtonEvent(3, mouse.XButton1 == ButtonState.Pressed);
        io.AddMouseButtonEvent(4, mouse.XButton2 == ButtonState.Pressed);

        io.AddMouseWheelEvent(
            (mouse.HorizontalScrollWheelValue - m_horizontalScrollWheelValue) / m_wheelDelta,
            (mouse.ScrollWheelValue - m_scrollWheelValue) / m_wheelDelta);
        m_scrollWheelValue = mouse.ScrollWheelValue;
        m_horizontalScrollWheelValue = mouse.HorizontalScrollWheelValue;

        foreach (var key in m_allKeys)
        {
            if (TryMapKeys(key, out ImGuiKey imguikey))
            {
                io.AddKeyEvent(imguikey, keyboard.IsKeyDown(key));
            }
        }

        io.DisplaySize = new System.Numerics.Vector2(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
        io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);
    }

    private bool TryMapKeys(Keys key, out ImGuiKey imguikey)
    {
        //Special case not handed in the switch...
        //If the actual key we put in is "None", return none and true. 
        //otherwise, return none and false.
        if (key == Keys.None)
        {
            imguikey = ImGuiKey.None;
            return true;
        }

        imguikey = key switch
        {
            Keys.Back => ImGuiKey.Backspace,
            Keys.Tab => ImGuiKey.Tab,
            Keys.Enter => ImGuiKey.Enter,
            Keys.CapsLock => ImGuiKey.CapsLock,
            Keys.Escape => ImGuiKey.Escape,
            Keys.Space => ImGuiKey.Space,
            Keys.PageUp => ImGuiKey.PageUp,
            Keys.PageDown => ImGuiKey.PageDown,
            Keys.End => ImGuiKey.End,
            Keys.Home => ImGuiKey.Home,
            Keys.Left => ImGuiKey.LeftArrow,
            Keys.Right => ImGuiKey.RightArrow,
            Keys.Up => ImGuiKey.UpArrow,
            Keys.Down => ImGuiKey.DownArrow,
            Keys.PrintScreen => ImGuiKey.PrintScreen,
            Keys.Insert => ImGuiKey.Insert,
            Keys.Delete => ImGuiKey.Delete,
            >= Keys.D0 and <= Keys.D9 => ImGuiKey._0 + (key - Keys.D0),
            >= Keys.A and <= Keys.Z => ImGuiKey.A + (key - Keys.A),
            >= Keys.NumPad0 and <= Keys.NumPad9 => ImGuiKey.Keypad0 + (key - Keys.NumPad0),
            Keys.Multiply => ImGuiKey.KeypadMultiply,
            Keys.Add => ImGuiKey.KeypadAdd,
            Keys.Subtract => ImGuiKey.KeypadSubtract,
            Keys.Decimal => ImGuiKey.KeypadDecimal,
            Keys.Divide => ImGuiKey.KeypadDivide,
            >= Keys.F1 and <= Keys.F12 => ImGuiKey.F1 + (key - Keys.F1),
            Keys.NumLock => ImGuiKey.NumLock,
            Keys.Scroll => ImGuiKey.ScrollLock,
            Keys.LeftShift => ImGuiKey.ModShift,
            Keys.LeftControl => ImGuiKey.ModCtrl,
            Keys.LeftAlt => ImGuiKey.ModAlt,
            Keys.OemSemicolon => ImGuiKey.Semicolon,
            Keys.OemPlus => ImGuiKey.Equal,
            Keys.OemComma => ImGuiKey.Comma,
            Keys.OemMinus => ImGuiKey.Minus,
            Keys.OemPeriod => ImGuiKey.Period,
            Keys.OemQuestion => ImGuiKey.Slash,
            Keys.OemTilde => ImGuiKey.GraveAccent,
            Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
            Keys.OemCloseBrackets => ImGuiKey.RightBracket,
            Keys.OemPipe => ImGuiKey.Backslash,
            Keys.OemQuotes => ImGuiKey.Apostrophe,
            _ => ImGuiKey.None,
        };

        return imguikey != ImGuiKey.None;
    }

    #endregion Setup & Update

    #region Internals

    /// <summary>
    /// Gets the geometry as set up by ImGui and sends it to the graphics device
    /// </summary>
    private void RenderDrawData(ImDrawDataPtr drawData)
    {
        // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
        var lastViewport = graphicsDevice.Viewport;
        var lastScissorBox = graphicsDevice.ScissorRectangle;

        graphicsDevice.BlendFactor = Color.White;
        graphicsDevice.BlendState = BlendState.NonPremultiplied;
        graphicsDevice.RasterizerState = m_rasterizerState;
        graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

        // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
        drawData.ScaleClipRects(ImGuiNET.ImGui.GetIO().DisplayFramebufferScale);

        // Setup projection
        graphicsDevice.Viewport = new Viewport(0, 0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);

        UpdateBuffers(drawData);

        RenderCommandLists(drawData);

        // Restore modified state
        graphicsDevice.Viewport = lastViewport;
        graphicsDevice.ScissorRectangle = lastScissorBox;
    }

    private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
    {
        if (drawData.TotalVtxCount == 0)
        {
            return;
        }

        // Expand buffers if we need more room
        if (drawData.TotalVtxCount > m_vertexBufferSize)
        {
            m_vertexBuffer?.Dispose();

            m_vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
            m_vertexBuffer = new VertexBuffer(graphicsDevice, DrawVertDeclaration.DECLARATION, m_vertexBufferSize, BufferUsage.None);
            m_vertexData = new byte[m_vertexBufferSize * DrawVertDeclaration.SIZE];
        }

        if (drawData.TotalIdxCount > m_indexBufferSize)
        {
            m_indexBuffer?.Dispose();

            m_indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
            m_indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, m_indexBufferSize, BufferUsage.None);
            m_indexData = new byte[m_indexBufferSize * sizeof(ushort)];
        }

        // Copy ImGui's vertices and indices to a set of managed byte arrays
        int vtxOffset = 0;
        int idxOffset = 0;

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];

            fixed (void* vtxDstPtr = &m_vertexData[vtxOffset * DrawVertDeclaration.SIZE])
            fixed (void* idxDstPtr = &m_indexData[idxOffset * sizeof(ushort)])
            {
                Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, m_vertexData.Length, cmdList.VtxBuffer.Size * DrawVertDeclaration.SIZE);
                Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, m_indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
            }

            vtxOffset += cmdList.VtxBuffer.Size;
            idxOffset += cmdList.IdxBuffer.Size;
        }

        // Copy the managed byte arrays to the gpu vertex- and index buffers
        m_vertexBuffer.SetData(m_vertexData, 0, drawData.TotalVtxCount * DrawVertDeclaration.SIZE);
        m_indexBuffer.SetData(m_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
    }

    private void RenderCommandLists(ImDrawDataPtr drawData)
    {
        graphicsDevice.SetVertexBuffer(m_vertexBuffer);
        graphicsDevice.Indices = m_indexBuffer;

        int vtxOffset = 0;
        int idxOffset = 0;

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];

            for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
            {
                ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];

                if (drawCmd.ElemCount == 0) 
                {
                    continue;
                }

                if (!m_loadedTextures.ContainsKey(drawCmd.TextureId))
                {
                    throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                }

                graphicsDevice.ScissorRectangle = new Rectangle(
                    (int)drawCmd.ClipRect.X,
                    (int)drawCmd.ClipRect.Y,
                    (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                    (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                );

                var effect = UpdateEffect(m_loadedTextures[drawCmd.TextureId]);

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    #pragma warning disable CS0618 // // FNA does not expose an alternative method.
                    graphicsDevice.DrawIndexedPrimitives(
                        primitiveType: PrimitiveType.TriangleList,
                        baseVertex: (int)drawCmd.VtxOffset + vtxOffset,
                        minVertexIndex: 0,
                        numVertices: cmdList.VtxBuffer.Size,
                        startIndex: (int)drawCmd.IdxOffset + idxOffset,
                        primitiveCount: (int)drawCmd.ElemCount / 3
                    );
                    #pragma warning restore CS0618
                }
            }

            vtxOffset += cmdList.VtxBuffer.Size;
            idxOffset += cmdList.IdxBuffer.Size;
        }
    }

    #endregion Internals
}

public static class DrawVertDeclaration
{
    public static readonly VertexDeclaration DECLARATION;

    public static readonly int SIZE;

    static DrawVertDeclaration()
    {
        unsafe { SIZE = sizeof(ImDrawVert); }

        DECLARATION = new VertexDeclaration(
            SIZE,

            // Position
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

            // UV
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

            // Color
            new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );
    }
}