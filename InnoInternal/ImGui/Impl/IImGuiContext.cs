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
    
    // Style
    enum StyleVar
    {
        Alpha,
        DisabledAlpha,
        WindowPadding,
        WindowRounding,
        WindowBorderSize,
        WindowMinSize,
        WindowTitleAlign,
        ChildRounding,
        ChildBorderSize,
        PopupRounding,
        PopupBorderSize,
        FramePadding,
        FrameRounding,
        FrameBorderSize,
        ItemSpacing,
        ItemInnerSpacing,
        IndentSpacing,
        CellPadding,
        ScrollbarSize,
        ScrollbarRounding,
        GrabMinSize,
        GrabRounding,
        TabRounding,
        TabBorderSize,
        TabBarBorderSize,
        TabBarOverlineSize,
        TableAngledHeadersAngle,
        TableAngledHeadersTextAlign,
        ButtonTextAlign,
        SelectableTextAlign,
        SeparatorTextBorderSize,
        SeparatorTextAlign,
        SeparatorTextPadding,
        DockingSeparatorSize,
        COUNT,
    }
    void PushStyleVar(StyleVar var, float indent);
    void PopStyleVar();
    
    // Tree
    [Flags]
    enum TreeNodeFlags
    {
        None = 0,
        Selected = 1,
        Framed = 2,
        AllowOverlap = 4,
        NoTreePushOnOpen = 8,
        NoAutoOpenOnLog = 16,           // 0x00000010
        DefaultOpen = 32,               // 0x00000020
        OpenOnDoubleClick = 64,         // 0x00000040
        OpenOnArrow = 128,              // 0x00000080
        Leaf = 256,                     // 0x00000100
        Bullet = 512,                   // 0x00000200
        FramePadding = 1024,            // 0x00000400
        SpanAvailWidth = 2048,          // 0x00000800
        SpanFullWidth = 4096,           // 0x00001000
        SpanTextWidth = 8192,           // 0x00002000
        SpanAllColumns = 16384,         // 0x00004000
        NavLeftJumpsBackHere = 32768,   // 0x00008000
        
        // Combined
        CollapsingHeader = NoAutoOpenOnLog | NoTreePushOnOpen | Framed, // 0x0000001A
    }
    bool TreeNode(string text, TreeNodeFlags flags = TreeNodeFlags.None);
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