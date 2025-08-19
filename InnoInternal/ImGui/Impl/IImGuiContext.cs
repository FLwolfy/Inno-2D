using InnoBase;
using InnoInternal.Render.Impl;

namespace InnoInternal.ImGui.Impl;

/// <summary>
/// A encapsulated immeadiate-mode GUI interface
/// </summary>
public interface IImGuiContext
{
    // Menu
    bool BeginMainMenuBar();
    void EndMainMenuBar();
    bool BeginMenu(string title, bool open = true);
    void EndMenu();
    bool MenuItem(string title);
    
    // Window
    bool BeginWindow(string title, bool open = true);
    void EndWindow();
    bool IsWindowHovered();
    bool IsWindowFocused();
    void SetWindowFocus();
    Vector2 GetContentRegionAvail();
    Vector2 GetWindowSize();
    Vector2 GetWindowPos();
    
    // Child
    [Flags]
    enum WindowFlags
    {
        None = 0,
        NoTitleBar = 1,
        NoResize = 2,
        NoMove = 4,
        NoScrollbar = 8,
        NoScrollWithMouse = 16, // 0x00000010
        NoCollapse = 32, // 0x00000020
        AlwaysAutoResize = 64, // 0x00000040
        NoBackground = 128, // 0x00000080
        NoSavedSettings = 256, // 0x00000100
        NoMouseInputs = 512, // 0x00000200
        MenuBar = 1024, // 0x00000400
        HorizontalScrollbar = 2048, // 0x00000800
        NoFocusOnAppearing = 4096, // 0x00001000
        NoBringToFrontOnFocus = 8192, // 0x00002000
        AlwaysVerticalScrollbar = 16384, // 0x00004000
        AlwaysHorizontalScrollbar = 32768, // 0x00008000
        NoNavInputs = 65536, // 0x00010000
        NoNavFocus = 131072, // 0x00020000
        UnsavedDocument = 262144, // 0x00040000
        NoDocking = 524288, // 0x00080000
        NoNav = NoNavFocus | NoNavInputs, // 0x00030000
        NoDecoration = NoCollapse | NoScrollbar | NoResize | NoTitleBar, // 0x0000002B
        NoInputs = NoNav | NoMouseInputs, // 0x00030200
        ChildWindow = 16777216, // 0x01000000
        Tooltip = 33554432, // 0x02000000
        Popup = 67108864, // 0x04000000
        Modal = 134217728, // 0x08000000
        ChildMenu = 268435456, // 0x10000000
        DockNodeHost = 536870912, // 0x20000000
    }
    [Flags]
    enum ChildFlags
    {
        None = 0,
        Borders = 1,
        AlwaysUseWindowPadding = 2,
        ResizeX = 4,
        ResizeY = 8,
        AutoResizeX = 16, // 0x00000010
        AutoResizeY = 32, // 0x00000020
        AlwaysAutoResize = 64, // 0x00000040
        FrameStyle = 128, // 0x00000080
        NavFlattened = 256, // 0x00000100
    }
    bool BeginChild(string name, Vector2 size = default, ChildFlags childFlags = ChildFlags.None);
    void EndChild();
     
    // Cursor
    Vector2 GetCursorStartPos();
    Vector2 GetCursorPos();
    Vector2 GetCursorScreenPos();
    void SetCursorPos(Vector2 pos);
    void SetCursorPosX(float x);
    void SetCursorPosY(float y);
    void SetCursorScreenPos(Vector2 pos);
    
    // Layout
    void BeginGroup();
    void EndGroup();
    void BeginInvisible();
    void EndInvisible();
    Vector2 GetInvisibleItemRectSize();
    void SameLine();
    float CalcItemWidth();
    Vector2 GetItemRectSize();
    void Dummy(Vector2 size);
    void Separator();
    
    // Widget
    void Text(string text);
    void BulletText(string text);
    void Image(ITexture texture, float width, float height);
    bool Selectable(string text);
    bool Button(string label);
    bool Checkbox(string label, ref bool value);
    bool SliderFloat(string label, ref float value, float min, float max);
    bool CollapsingHeader(string compName, TreeNodeFlags flags = TreeNodeFlags.None);
    bool CollapsingHeader(string compName, ref bool visible, TreeNodeFlags flags = TreeNodeFlags.None);
    bool Combo(string label, ref int selectedIndex, string[] list);
    
    // Input
    bool InputInt(string label, ref int value);
    bool InputFloat(string label, ref float value);
    bool InputFloat2(string label, ref Vector2 value);
    bool InputFloat3(string label, ref Vector3 value);
    bool InputQuaternion(string label, ref Quaternion value);
    bool InputText(string label, ref string value, uint maxLength);
    bool ColorEdit4(string label, in Color input, out Color output);
    
    // Event
    void PushID(int id);
    void PopID();
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
    bool IsAnyItemHovered();
    
    // Disable
    void BeginDisabled();
    void EndDisabled();
    
    // Table 
    [Flags]
    enum TableFlags
    {
        None = 0,
        Resizable = 1,
        Reorderable = 2,
        Hideable = 4,
        Sortable = 8,
        NoSavedSettings = 16, // 0x00000010
        ContextMenuInBody = 32, // 0x00000020
        RowBg = 64, // 0x00000040
        BordersInnerH = 128, // 0x00000080
        BordersOuterH = 256, // 0x00000100
        BordersInnerV = 512, // 0x00000200
        BordersOuterV = 1024, // 0x00000400
        BordersH = BordersOuterH | BordersInnerH, // 0x00000180
        BordersV = BordersOuterV | BordersInnerV, // 0x00000600
        BordersInner = BordersInnerV | BordersInnerH, // 0x00000280
        BordersOuter = BordersOuterV | BordersOuterH, // 0x00000500
        Borders = BordersOuter | BordersInner, // 0x00000780
        NoBordersInBody = 2048, // 0x00000800
        NoBordersInBodyUntilResize = 4096, // 0x00001000
        SizingFixedFit = 8192, // 0x00002000
        SizingFixedSame = 16384, // 0x00004000
        SizingStretchProp = SizingFixedSame | SizingFixedFit, // 0x00006000
        SizingStretchSame = 32768, // 0x00008000
        NoHostExtendX = 65536, // 0x00010000
        NoHostExtendY = 131072, // 0x00020000
        NoKeepColumnsVisible = 262144, // 0x00040000
        PreciseWidths = 524288, // 0x00080000
        NoClip = 1048576, // 0x00100000
        PadOuterX = 2097152, // 0x00200000
        NoPadOuterX = 4194304, // 0x00400000
        NoPadInnerX = 8388608, // 0x00800000
        ScrollX = 16777216, // 0x01000000
        ScrollY = 33554432, // 0x02000000
        SortMulti = 67108864, // 0x04000000
        SortTristate = 134217728, // 0x08000000
        HighlightHoveredColumn = 268435456, // 0x10000000
        SizingMask = SizingStretchSame | SizingStretchProp, // 0x0000E000
    }
    void BeginTable(string text, int columnCount, TableFlags flags);
    void EndTable();
    void TableNextRow();
    void TableNextColumn();
    void TableSetColumnIndex(int index);
    
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
    
    // Popup
    bool BeginPopup(string label);
    bool BeginPopupContextItem(string label);
    void EndPopup();
    void OpenPopup(string label);
    void CloseCurrentPopup();
    
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