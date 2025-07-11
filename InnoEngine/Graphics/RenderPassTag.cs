namespace InnoEngine.Graphics;

/// <summary>
/// Defines the order of rendering passes.
/// </summary>
internal enum RenderPassTag
{
    ClearScreen,
    Background,
    Sprite,
    Lighting,
    PostProcessing,
    UI
}