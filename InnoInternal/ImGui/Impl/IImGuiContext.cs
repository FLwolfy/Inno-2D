using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.ImGui.Impl;

/// <summary>
/// A encapsulated immeadiate-mode GUI interface
/// </summary>
internal interface IImGuiContext
{
    bool BeginMainMenuBar();
    void EndMainMenuBar();
    
    bool BeginWindow(string title, bool open = true);
    void EndWindow();
    
    bool BeginMenu(string title, bool open = true);
    void EndMenu();

    Vector2 GetContentRegionAvail();

    // RenderTarget
    void Image(ITexture2D texture, float width, float height);
    
    // Widget
    void Text(string text);
    bool Button(string label);
    void Checkbox(string label, ref bool value);
    void SliderFloat(string label, ref float value, float min, float max);
    
    // IO
    bool IsWindowHovered();
    Vector2 GetWindowPosition();
    Vector2 GetCursorScreenPos();
    bool IsMouseDown(int button);
    Vector2 GetMouseDelta();
    Vector2 GetMousePosition();
    float GetMouseWheel();
}