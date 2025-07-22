using InnoBase;
using InnoInternal.Resource.Impl;

namespace InnoInternal.ImGui.Impl;

/// <summary>
/// A encapsulated immeadiate-mode GUI interface
/// </summary>
internal interface IImGuiContext
{
    // Menu
    bool BeginMainMenuBar();
    void EndMainMenuBar();
    bool BeginMenu(string title, bool open = true);
    void EndMenu();
    
    // Window
    bool BeginWindow(string title, bool open = true);
    void EndWindow();
    bool IsWindowHovered();
    bool IsWindowFocused();
    void SetWindowFocus();
    Vector2 GetContentRegionAvail();
    Vector2 GetWindowPosition();
    Vector2 GetCursorStartPos();
    
    // Widget
    void Image(ITexture2D texture, float width, float height);
    void Text(string text);
    bool Selectable(string text);
    void BulletText(string text);
    bool Button(string label);
    void Checkbox(string label, ref bool value);
    void SliderFloat(string label, ref float value, float min, float max);
    
    // Tree
    bool TreeNode(string text);
    void TreePop();
    
    // Drag & Drop
    bool BeginDragDropSource();
    void SetDragDropPayload<T>(string type, T data) where T : unmanaged;
    T? AcceptDragDropPayload<T>(string type) where T : unmanaged;
    void EndDragDropSource();
    bool BeginDragDropTarget();
    void EndDragDropTarget();

    
    // Gizmos Overlay
    void DrawLine(Vector2 p1, Vector2 p2, Color color, float thickness = 1f);
    void DrawText(Vector2 pos, string text, Color color);
    
    // IO
    bool IsMouseDown(int button);
    bool IsMouseClicked(int button);
    float GetMouseWheel();
    
    Vector2 GetMouseDelta();
    Vector2 GetMousePosition();
    
}