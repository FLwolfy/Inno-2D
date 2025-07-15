using InnoInternal.Resource.Impl;

namespace InnoInternal.ImGui.Impl;

/// <summary>
/// A encapsulated immeadiate-mode GUI interface
/// </summary>
internal interface IImGuiContext
{
    void BeginWindow(string title, bool open = true);
    void EndWindow();

    // RenderTarget
    void Image(ITexture2D texture, float width, float height);
    
    // Widget
    void Text(string text);
    bool Button(string label);
    void Checkbox(string label, ref bool value);
    void SliderFloat(string label, ref float value, float min, float max);

    // Docking
    void DockSpace();
    bool IsWindowHovered();
    float GetMouseWheel();
}