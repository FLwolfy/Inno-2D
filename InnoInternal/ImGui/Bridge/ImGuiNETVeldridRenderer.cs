using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

using Veldrid;

using ImGuiNET;
using InnoInternal.Render.Bridge;
using InnoInternal.Shell.Bridge;
using InnoInternal.Shell.Impl;
using SYSVector4 = System.Numerics.Vector4;

namespace InnoInternal.ImGui.Bridge;

internal class ImGuiNETVeldridRenderer : IImGuiRenderer
{
    // Graphics
    private GraphicsDevice m_graphicsDevice = null!;
    private VeldridSdl2Window m_veldridWindow = null!;
    
    // Resources
    private CommandList m_commandList = null!;
    private ImGuiNETVeldridController m_imGuiVeldridController = null!;
    
    // Properties
    public IImGuiContext context { get; private set; } = null!;
    public IntPtr mainMainContextPtr { get; private set; }
    public IntPtr virtualContextPtr { get; private set; }
    
    public unsafe void Initialize(IGraphicsDevice graphicsDevice, IWindow windowHolder)
    {
        if (graphicsDevice is not VeldridGraphicsDevice device)
            throw new ArgumentException("Expected a Veldrid GraphicsDevice.", nameof(graphicsDevice));
        if (windowHolder is not VeldridSdl2Window window)
            throw new ArgumentException("Expected a Sdl2Window.", nameof(windowHolder));
        
        m_graphicsDevice = device.inner;
        m_commandList = m_graphicsDevice.ResourceFactory.CreateCommandList();
        m_veldridWindow = window;
        m_imGuiVeldridController = new ImGuiNETVeldridController(
            m_graphicsDevice,
            m_veldridWindow.inner,
            m_graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            ImGuiNETColorSpaceHandling.Legacy
        );
        
        // Main Context
        mainMainContextPtr = ImGuiNET.ImGui.GetCurrentContext();
        ImGuiNET.ImGui.SetCurrentContext(mainMainContextPtr);
        
        // Virtual Context
        virtualContextPtr = ImGuiNET.ImGui.CreateContext(ImGuiNET.ImGui.GetIO().Fonts.NativePtr);
        
        // ImGui Context Wrapper
        context = new ImGuiNETContext(this);
        
        // Setups
        SetupThemes();
    }

    public void SwapExtraImGuiWindows()
    {
        m_imGuiVeldridController.SwapExtraWindowBuffers(m_graphicsDevice);
    }

    public void BeginLayout(float deltaTime, IFrameBuffer frameBuffer)
    {
        var buffer = frameBuffer as VeldridFrameBuffer;
        if (buffer == null) throw new ArgumentException("Expected a VeldridFrameBuffer.", nameof(frameBuffer));
        
        // Begin Render
        m_commandList.Begin();
        m_commandList.SetFramebuffer(buffer.inner);
        
        // Virtual Context
        ImGuiNET.ImGui.SetCurrentContext(virtualContextPtr);
        ImGuiNET.ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(m_veldridWindow.width, m_veldridWindow.height);
        ImGuiNET.ImGui.NewFrame();
        
        // Main Context
        ImGuiNET.ImGui.SetCurrentContext(mainMainContextPtr);
        m_imGuiVeldridController.Update(deltaTime, m_veldridWindow.innerSnapshot, m_imGuiVeldridController.PumpExtraWindowInputs());
        
        // Docking
        ImGuiNET.ImGui.DockSpaceOverViewport(ImGuiNET.ImGui.GetMainViewport().ID);
    }

    public void EndLayout()
    {
        // Virtual Context
        ImGuiNET.ImGui.SetCurrentContext(virtualContextPtr);
        ImGuiNET.ImGui.EndFrame();
        
        // Main Context
        ImGuiNET.ImGui.SetCurrentContext(mainMainContextPtr);
        
        // Render
        m_imGuiVeldridController.Render(m_graphicsDevice, m_commandList);
        m_commandList.End();
        m_graphicsDevice.SubmitCommands(m_commandList);
    }

    public IntPtr BindTexture(ITexture texture)
    {
        if (texture is not VeldridTexture veldridTexture)
            throw new ArgumentException("Expected a Veldrid Texture.", nameof(texture));
        
        return m_imGuiVeldridController.GetOrCreateImGuiBinding(m_graphicsDevice.ResourceFactory, veldridTexture.inner);
    }
    
    public void UnbindTexture(ITexture texture)
    {
        if (texture is not VeldridTexture veldridTexture)
            throw new ArgumentException("Expected a Veldrid Texture.", nameof(texture));
        
        m_imGuiVeldridController.RemoveImGuiBinding(veldridTexture.inner);
    }
    
    private void SetupThemes()
    {
        ImGuiNET.ImGui.StyleColorsDark();
        
        var style = ImGuiNET.ImGui.GetStyle();
        var colors = style.Colors;

        // Window Background
        colors[(int)ImGuiCol.WindowBg] = new SYSVector4(0.10f, 0.10f, 0.11f, 1.0f);

        // Headers
        colors[(int)ImGuiCol.Header] = new SYSVector4(0.20f, 0.205f, 0.25f, 1.0f);
        colors[(int)ImGuiCol.HeaderHovered] = new SYSVector4(0.35f, 0.30f, 0.45f, 1.0f); // muted purple
        colors[(int)ImGuiCol.HeaderActive] = new SYSVector4(0.30f, 0.25f, 0.40f, 1.0f);

        // Buttons
        colors[(int)ImGuiCol.Button] = new SYSVector4(0.20f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.ButtonHovered] = new SYSVector4(0.35f, 0.30f, 0.45f, 1.0f);
        colors[(int)ImGuiCol.ButtonActive] = new SYSVector4(0.30f, 0.25f, 0.40f, 1.0f);

        // Frame BG
        colors[(int)ImGuiCol.FrameBg] = new SYSVector4(0.18f, 0.18f, 0.20f, 1.0f);
        colors[(int)ImGuiCol.FrameBgHovered] = new SYSVector4(0.35f, 0.30f, 0.45f, 1.0f);
        colors[(int)ImGuiCol.FrameBgActive] = new SYSVector4(0.30f, 0.25f, 0.40f, 1.0f);

        // Tabs
        colors[(int)ImGuiCol.Tab] = new SYSVector4(0.13f, 0.13f, 0.16f, 1.0f);
        colors[(int)ImGuiCol.TabHovered] = new SYSVector4(0.45f, 0.35f, 0.60f, 1.0f);
        colors[(int)ImGuiCol.TabSelected] = new SYSVector4(0.38f, 0.32f, 0.50f, 1.0f);
        colors[(int)ImGuiCol.TabDimmed] = new SYSVector4(0.10f, 0.10f, 0.12f, 1.0f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new SYSVector4(0.28f, 0.23f, 0.36f, 1.0f);

        // Title bar
        colors[(int)ImGuiCol.TitleBg] = new SYSVector4(0.12f, 0.12f, 0.15f, 1.0f);
        colors[(int)ImGuiCol.TitleBgActive] = new SYSVector4(0.18f, 0.18f, 0.22f, 1.0f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new SYSVector4(0.08f, 0.08f, 0.10f, 1.0f);

        // Optional: Resize grip, scrollbar, etc. for more polish
        colors[(int)ImGuiCol.ResizeGrip] = new SYSVector4(0.3f, 0.3f, 0.35f, 0.6f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new SYSVector4(0.4f, 0.35f, 0.5f, 0.7f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new SYSVector4(0.30f, 0.30f, 0.35f, 1.0f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new SYSVector4(0.40f, 0.35f, 0.50f, 1.0f);
        colors[(int)ImGuiCol.CheckMark] = new SYSVector4(0.7f, 0.6f, 0.9f, 1.0f);

        // Style tweaks
        style.WindowRounding = 4.0f;
        style.FrameRounding = 3.0f;
        style.ScrollbarRounding = 3.0f;
        style.GrabRounding = 3.0f;
    }
    
    public void Dispose()
    {
        m_commandList.Dispose();
        m_imGuiVeldridController.Dispose();
    }
}

