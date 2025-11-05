using InnoInternal.ImGui.Bridge;
using InnoInternal.Render.Impl;

namespace InnoInternal.ImGui.Impl;

/// <summary>
/// Interface for ImGui renderer abstraction.
/// Responsible for handling frame lifecycle, rendering ImGui draw data,
/// and binding textures for use in ImGui.
/// </summary>
public interface IImGuiRenderer
{
    void Initialize(IGraphicsDevice graphicsDevice, object windowHolder);

    /// <summary>
    /// Swaps extra windows between the main and virtual ImGui contexts.
    /// </summary>
    void SwapExtraImGuiWindows();
    
    /// <summary>
    /// Starts a new ImGui frame. Should be called before any ImGui calls each frame.
    /// </summary>
    void BeginLayout(float deltaTime);

    /// <summary>
    /// Ends the ImGui frame and finalizes draw data.
    /// </summary>
    void EndLayout();
    
    /// <summary>
    /// Handles resizing of the window.
    /// </summary>
    void OnWindowResize(int width, int height);

    /// <summary>
    /// Binds a texture for use by ImGui and returns a texture ID handle.
    /// </summary>
    /// <param name="texture">The texture to bind.</param>
    /// <returns>An IntPtr handle used by ImGui to reference this texture.</returns>
    IntPtr BindTexture(ITexture texture);
    
    /// <summary>
    /// Unbinds a previously bound texture from ImGui.
    /// </summary>
    /// <param name="texture">The texture to unbind.</param>
    void UnbindTexture(ITexture texture);
    
    /// <summary>
    /// Gets the associated ImGui context abstraction.
    /// </summary>
    IImGuiContext context { get; }
    
    /// <summary>
    /// Gets the pointer to the main ImGui context.
    /// </summary>
    IntPtr mainMainContextPtr { get; }
    
    /// <summary>
    /// Gets the pointer to the virtual ImGui context.
    /// </summary>
    IntPtr virtualContextPtr { get; }
    
    // Create Shell
    enum ImGuiRendererType { Veldrid }
    
    static IImGuiRenderer CreateRenderer(ImGuiRendererType rendererType)
    {
        return rendererType switch
        {
            ImGuiRendererType.Veldrid => new ImGuiNETVeldridRenderer(),
            
            // Default case to handle unsupported shell types
            _ => throw new NotSupportedException($"ImGuiRenderer type {rendererType} is not supported.")
        };
    }
}