using InnoInternal.Resource.Impl;

namespace InnoInternal.ImGui.Impl;

/// <summary>
/// Interface for ImGui renderer abstraction.
/// Responsible for handling frame lifecycle, rendering ImGui draw data,
/// and binding textures for use in ImGui.
/// </summary>
internal interface IImGuiRenderer : IDisposable
{
    void Initialize();
    
    /// <summary>
    /// Starts a new ImGui frame. Should be called before any ImGui calls each frame.
    /// </summary>
    void BeginFrame();

    /// <summary>
    /// Ends the ImGui frame and finalizes draw data.
    /// </summary>
    void EndFrame();

    /// <summary>
    /// Renders the ImGui draw data.
    /// The parameter is loosely typed as object to allow abstraction.
    /// </summary>
    /// <param name="drawData">The draw data object, typically ImDrawDataPtr or a wrapper.</param>
    void RenderDrawData(object drawData);

    /// <summary>
    /// Binds a texture for use by ImGui and returns a texture ID handle.
    /// </summary>
    /// <param name="texture">The texture to bind.</param>
    /// <returns>An IntPtr handle used by ImGui to reference this texture.</returns>
    IntPtr BindTexture(ITexture2D texture);
    
    /// <summary>
    /// Gets the associated ImGui context abstraction.
    /// </summary>
    IImGuiContext context { get; }
}