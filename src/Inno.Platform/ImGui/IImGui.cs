using Inno.Platform.Graphics;
using Inno.Platform.Graphics.Bridge;
using Inno.Platform.ImGui.Bridge;
using Inno.Platform.Window;
using Inno.Platform.Window.Bridge;

namespace Inno.Platform.ImGui;

/// <summary>
/// Identifies the kind of color space handling that ImGui uses.
/// </summary>
public enum ImGuiColorSpaceHandling
{
    /// <summary>
    /// Legacy-style color space handling. In this mode, the renderer will not convert sRGB vertex colors into linear space
    /// before blending them.
    /// </summary>
    Legacy = 0,
    /// <summary>
    /// Improved color space handling. In this mode, the render will convert sRGB vertex colors into linear space before
    /// blending them with colors from user Textures.
    /// </summary>
    Linear = 1,
}

/// <summary>
/// Interface for ImGui renderer abstraction.
/// Responsible for handling frame lifecycle, rendering ImGui draw data,
/// and binding textures for use in ImGui.
/// </summary>
public interface IImGui : IDisposable
{
    /// <summary>
    /// Starts a new ImGui frame. Should be called before any ImGui calls each frame.
    /// </summary>
    void BeginLayout(float deltaTime);

    /// <summary>
    /// Ends the ImGui frame and finalizes draw data.
    /// </summary>
    void EndLayout();
    
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
    /// Gets the pointer to the main ImGui context.
    /// </summary>
    IntPtr mainMainContextPtr { get; }
    
    /// <summary>
    /// Gets the pointer to the virtual ImGui context.
    /// </summary>
    IntPtr virtualContextPtr { get; }
}