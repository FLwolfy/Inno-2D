namespace InnoEngine.Graphics;

/// <summary>
/// Defines the order of rendering passes.
/// </summary>
public enum RenderPassTag
{
    ClearScreen,
    Background,
    World,
    Lighting,
    PostProcessing,
    UI
}