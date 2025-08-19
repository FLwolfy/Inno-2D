using InnoBase;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoInternal.ImGui.Bridge;

internal class ImGuiNETContext : IImGuiContext
{
    private readonly IImGuiRenderer m_renderer;
    
    private bool m_inInvisible = false;
    private Vector2 m_invisibleSizeCache = Vector2.ZERO;

    internal ImGuiNETContext(IImGuiRenderer renderer)
    {
        m_renderer = renderer;
    }

    // Menu
    public bool BeginMainMenuBar() => ImGuiNET.ImGui.BeginMainMenuBar();
    public void EndMainMenuBar() => ImGuiNET.ImGui.EndMainMenuBar();
    public bool BeginMenu(string title, bool open = true) => ImGuiNET.ImGui.BeginMenu(title, open);
    public void EndMenu() => ImGuiNET.ImGui.EndMenu();
    public bool MenuItem(string title) => ImGuiNET.ImGui.MenuItem(title);

    // Window
    public bool BeginWindow(string title, bool open = true) => ImGuiNET.ImGui.Begin(title, ref open);
    public void EndWindow() => ImGuiNET.ImGui.End();
    public bool IsWindowHovered() => ImGuiNET.ImGui.IsWindowHovered();
    public bool IsWindowFocused() =>  ImGuiNET.ImGui.IsWindowFocused();
    public void SetWindowFocus() => ImGuiNET.ImGui.SetWindowFocus();

    public Vector2 GetContentRegionAvail() => new Vector2(ImGuiNET.ImGui.GetContentRegionAvail().X, ImGuiNET.ImGui.GetContentRegionAvail().Y);
    public Vector2 GetWindowSize() => new Vector2(ImGuiNET.ImGui.GetWindowSize().X, ImGuiNET.ImGui.GetWindowSize().Y);
    public Vector2 GetWindowPos() => new Vector2(ImGuiNET.ImGui.GetWindowPos().X, ImGuiNET.ImGui.GetWindowPos().Y);
    public Vector2 GetCursorStartPos() => new Vector2(ImGuiNET.ImGui.GetCursorStartPos().X, ImGuiNET.ImGui.GetCursorStartPos().Y);
    public Vector2 GetCursorPos() => new Vector2(ImGuiNET.ImGui.GetCursorPos().X, ImGuiNET.ImGui.GetCursorPos().Y);
    public Vector2 GetCursorScreenPos() => new Vector2(ImGuiNET.ImGui.GetCursorScreenPos().X, ImGuiNET.ImGui.GetCursorScreenPos().Y);
    public void SetCursorPos(Vector2 pos) => ImGuiNET.ImGui.SetCursorPos(new System.Numerics.Vector2(pos.x, pos.y));
    public void SetCursorPosX(float x) => ImGuiNET.ImGui.SetCursorPosX(x);
    public void SetCursorPosY(float y) => ImGuiNET.ImGui.SetCursorPosY(y);
    public void SetCursorScreenPos(Vector2 pos) => ImGuiNET.ImGui.SetCursorScreenPos(new System.Numerics.Vector2(pos.x, pos.y));

    // Layout
    public void BeginGroup() => ImGuiNET.ImGui.BeginGroup();
    public void EndGroup() => ImGuiNET.ImGui.EndGroup();
    public void BeginInvisible()
    {
        if (m_inInvisible) throw new InvalidOperationException("Cannot nest invisible groups.");
        m_inInvisible = true;

        System.Numerics.Vector2 currentAvailSize = ImGuiNET.ImGui.GetContentRegionAvail();
        
        ImGuiNET.ImGui.SetCurrentContext(m_renderer.virtualContextPtr);
        ImGuiNET.ImGui.PushID("INVISIBLE_ID");
        ImGuiNET.ImGui.BeginChild("INVISIBLE_GROUP", currentAvailSize);
        ImGuiNET.ImGui.BeginGroup();
    }
    public void EndInvisible()
    {
        ImGuiNET.ImGui.EndGroup();
        m_invisibleSizeCache = new Vector2(ImGuiNET.ImGui.GetItemRectSize().X, ImGuiNET.ImGui.GetItemRectSize().Y);
        ImGuiNET.ImGui.EndChild();
        ImGuiNET.ImGui.PopID();
        ImGuiNET.ImGui.SetCurrentContext(m_renderer.mainMainContextPtr);
        
        m_inInvisible = false;
    }
    public Vector2 GetInvisibleItemRectSize() => m_invisibleSizeCache;
    public void SameLine() => ImGuiNET.ImGui.SameLine();
    public float CalcItemWidth() => ImGuiNET.ImGui.CalcItemWidth();
    public Vector2 GetItemRectSize() => new Vector2(ImGuiNET.ImGui.GetItemRectSize().X, ImGuiNET.ImGui.GetItemRectSize().Y);
    public void Dummy(Vector2 size) => ImGuiNET.ImGui.Dummy(new System.Numerics.Vector2(size.x, size.y));
    public void Separator() => ImGuiNET.ImGui.Separator();

    // Child
    public bool BeginChild(string name, Vector2 size = default, IImGuiContext.ChildFlags childFlags = IImGuiContext.ChildFlags.None) => ImGuiNET.ImGui.BeginChild(name, new System.Numerics.Vector2(size.x, size.y), (ImGuiNET.ImGuiChildFlags)(int)childFlags);
    public void EndChild() => ImGuiNET.ImGui.EndChild();
    
    // Widget
    public void Text(string text) => ImGuiNET.ImGui.Text(text);
    public void BulletText(string text) => ImGuiNET.ImGui.BulletText(text);
    public void Image(ITexture texture, float width, float height) => ImGuiNET.ImGui.Image(m_renderer.BindTexture(texture), new System.Numerics.Vector2(width, height));
    public bool Selectable(string text) => ImGuiNET.ImGui.Selectable(text);
    public bool Button(string label) => ImGuiNET.ImGui.Button(label);
    public bool Checkbox(string label, ref bool value) => ImGuiNET.ImGui.Checkbox(label, ref value);
    public bool SliderFloat(string label, ref float value, float min, float max) => ImGuiNET.ImGui.SliderFloat(label, ref value, min, max);
    public bool CollapsingHeader(string compName, IImGuiContext.TreeNodeFlags flags = IImGuiContext.TreeNodeFlags.None) => ImGuiNET.ImGui.CollapsingHeader(compName, (ImGuiNET.ImGuiTreeNodeFlags)flags);
    public bool CollapsingHeader(string compName, ref bool visible, IImGuiContext.TreeNodeFlags flags = IImGuiContext.TreeNodeFlags.None) => ImGuiNET.ImGui.CollapsingHeader(compName, ref visible, (ImGuiNET.ImGuiTreeNodeFlags)flags);
    public bool Combo(string label, ref int selectedIndex, string[] list) => ImGuiNET.ImGui.Combo(label, ref selectedIndex, list, list.Length);

    // Input
    public bool InputInt(string label, ref int value) => ImGuiNET.ImGui.InputInt(label, ref value);
    public bool InputFloat(string label, ref float value) => ImGuiNET.ImGui.InputFloat(label, ref value);
    public bool InputFloat2(string label, ref Vector2 value)
    {
        System.Numerics.Vector2 value2 = new(value.x, value.y);
        bool result = ImGuiNET.ImGui.InputFloat2(label, ref value2);
        value.x = value2.X;
        value.y = value2.Y;
        return result;
    }
    public bool InputFloat3(string label, ref Vector3 value)
    {
        System.Numerics.Vector3 value2 = new(value.x, value.y, value.z);
        bool result = ImGuiNET.ImGui.InputFloat3(label, ref value2);
        value.x = value2.X;
        value.y = value2.Y;
        value.z = value2.Z;
        return result;
    }
    public bool InputQuaternion(string label, ref Quaternion value)   
    {
        System.Numerics.Vector4 value2 = new(value.x, value.y, value.z, value.w);
        bool result = ImGuiNET.ImGui.InputFloat4(label, ref value2);
        value.x = value2.X;
        value.y = value2.Y;
        value.z = value2.Z;
        value.w = value2.W;
        return result;
    }
    public bool InputText(string label, ref string value, uint maxLength) => ImGuiNET.ImGui.InputText(label, ref value, maxLength);
    public bool ColorEdit4(string label, in Color input, out Color output)
    {
        System.Numerics.Vector4 value2 = new(input.r, input.g, input.b, input.a);
        bool result = ImGuiNET.ImGui.ColorEdit4(label, ref value2);
        output = new Color(value2.X, value2.Y, value2.Z, value2.W);
        return result;
    }
    
    // Style
    public void PushStyleVar(IImGuiContext.StyleVar var, float indent) => ImGuiNET.ImGui.PushStyleVar((ImGuiNET.ImGuiStyleVar)(int)var, indent);
    public void PopStyleVar() => ImGuiNET.ImGui.PopStyleVar();
    
    // Item Flags
    public void PushItemFlag(IImGuiContext.ItemFlags flags, bool enabled) => ImGuiNET.ImGui.PushItemFlag((ImGuiNET.ImGuiItemFlags)flags, enabled);
    public void PopItemFlag() => ImGuiNET.ImGui.PopItemFlag();
    public bool IsAnyItemHovered() => ImGuiNET.ImGui.IsAnyItemHovered();
    
    // Disable
    public void BeginDisabled() => ImGuiNET.ImGui.BeginDisabled();
    public void EndDisabled() => ImGuiNET.ImGui.EndDisabled();

    // Event
    public void PushID(int id) => ImGuiNET.ImGui.PushID(id);
    public void PopID() => ImGuiNET.ImGui.PopID();
    public bool IsItemClicked(int button) => ImGuiNET.ImGui.IsItemClicked((ImGuiNET.ImGuiMouseButton)button);
    
    // Table
    public void BeginTable(string text, int columnCount, IImGuiContext.TableFlags flags) => ImGuiNET.ImGui.BeginTable(text, columnCount, (ImGuiNET.ImGuiTableFlags)flags);
    public void EndTable() => ImGuiNET.ImGui.EndTable();
    public void TableNextRow() => ImGuiNET.ImGui.TableNextRow();
    public void TableNextColumn() => ImGuiNET.ImGui.TableNextColumn();
    public void TableSetColumnIndex(int index) => ImGuiNET.ImGui.TableSetColumnIndex(index);

    // Tree
    public bool TreeNode(string text, IImGuiContext.TreeNodeFlags flags = IImGuiContext.TreeNodeFlags.None) => ImGuiNET.ImGui.TreeNodeEx(text, (ImGuiNET.ImGuiTreeNodeFlags)flags);
    
    public void TreePop() => ImGuiNET.ImGui.TreePop();
    
    // Popup
    public bool BeginPopup(string label) => ImGuiNET.ImGui.BeginPopup(label);
    public bool BeginPopupContextItem(string label) => ImGuiNET.ImGui.BeginPopupContextItem(label);
    public void EndPopup() => ImGuiNET.ImGui.EndPopup();
    public void OpenPopup(string label) => ImGuiNET.ImGui.OpenPopup(label);
    public void CloseCurrentPopup() => ImGuiNET.ImGui.CloseCurrentPopup();
    
    // Drag & Drop
    public bool BeginDragDropSource() => ImGuiNET.ImGui.BeginDragDropSource();
    public unsafe void SetDragDropPayload<T>(string type, T data) where T : unmanaged
    {
        T* ptr = &data;
        ImGuiNET.ImGui.SetDragDropPayload(type, (IntPtr)ptr, (uint)sizeof(T));
    }
    public unsafe T? AcceptDragDropPayload<T>(string type) where T : unmanaged
    {
        var payload = ImGuiNET.ImGui.AcceptDragDropPayload(type);
        if (payload.NativePtr == null || payload.Data == IntPtr.Zero) { return null; }
        return *(T*)payload.Data.ToPointer();
    }
    public void EndDragDropSource() => ImGuiNET.ImGui.EndDragDropSource();
    public bool BeginDragDropTarget() => ImGuiNET.ImGui.BeginDragDropTarget();
    public void EndDragDropTarget() =>  ImGuiNET.ImGui.EndDragDropTarget();
    

    // Gizmos Overlay
    public void DrawLine(Vector2 p1, Vector2 p2, Color color, float thickness = 1f)
    {
        var drawList = ImGuiNET.ImGui.GetWindowDrawList();
        drawList.AddLine(new System.Numerics.Vector2(p1.x, p1.y), new System.Numerics.Vector2(p2.x, p2.y), color.ToUInt32ARGB(), thickness);
    }

    public void DrawText(Vector2 pos, string text, Color color)
    {
        var drawList = ImGuiNET.ImGui.GetWindowDrawList();
        drawList.AddText(new System.Numerics.Vector2(pos.x, pos.y), color.ToUInt32ARGB(), text);
    }
    
    // IO
    public bool IsMouseDown(int button) => ImGuiNET.ImGui.GetIO().MouseDown[button];
    public bool IsMouseClicked(int button) => ImGuiNET.ImGui.GetIO().MouseClicked[button];
    public float GetMouseWheel() => ImGuiNET.ImGui.GetIO().MouseWheel;
    public Vector2 GetMouseDelta() => new Vector2(ImGuiNET.ImGui.GetIO().MouseDelta.X, ImGuiNET.ImGui.GetIO().MouseDelta.Y);
    public Vector2 GetMousePosition() => new Vector2(ImGuiNET.ImGui.GetIO().MousePos.X, ImGuiNET.ImGui.GetIO().MousePos.Y);
}