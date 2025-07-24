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
    Vector2 GetWindowSize();
    Vector2 GetWindowPos();
    
    // Cursor
    Vector2 GetCursorStartPos();
    void SetCursorPosX(float x);
    void SetCursorPosY(float y);
    
    // Layout
    void BeginGroup();
    void EndGroup();
    void SameLine();
    float CalcItemWidth();
    float CalcItemHeight();
    
    // Widget
    void Text(string text);
    void BulletText(string text);
    void Image(ITexture2D texture, float width, float height);
    bool Selectable(string text);
    bool Button(string label);
    bool Checkbox(string label, ref bool value);
    bool SliderFloat(string label, ref float value, float min, float max);
    bool CollapsingHeader(string compName, ref bool visible, TreeNodeFlags flags = TreeNodeFlags.None);
    
    // Input
    bool InputInt(string label, ref int value);
    bool InputFloat(string label, ref float value);
    bool InputFloat2(string label, ref Vector2 value);
    bool InputFloat3(string label, ref Vector3 value);
    bool InputQuaternion(string label, ref Quaternion value);
    bool InputText(string label, ref string value, uint maxLength);
    bool ColorEdit4(string label, in Color input, out Color output);
    
    // Event
    bool IsItemClicked(int button);
    
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

    // Item Flags
    [Flags]
    public enum ItemFlags
    {
        None = 0,
        NoTabStop = 1,
        NoNav = 2,
        NoNavDefaultFocus = 4,
        ButtonRepeat = 8,
        AutoClosePopups = 16, // 0x00000010
        AllowDuplicateId = 32, // 0x00000020
    }
    void PushItemFlag(ItemFlags flags, bool enabled);
    void PopItemFlag();
    
    // Disable
    void BeginDisabled();
    void EndDisabled();
    
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