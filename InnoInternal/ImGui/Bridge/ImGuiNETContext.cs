using InnoBase;
using InnoInternal.ImGui.Impl;
using InnoInternal.Resource.Impl;

namespace InnoInternal.ImGui.Bridge;

internal class ImGuiNETContext : IImGuiContext
{
    private readonly IImGuiRenderer m_renderer;

    internal ImGuiNETContext(IImGuiRenderer renderer)
    {
        m_renderer = renderer;
    }

    // Menu
    public bool BeginMainMenuBar() => ImGuiNET.ImGui.BeginMainMenuBar();
    public void EndMainMenuBar() => ImGuiNET.ImGui.EndMainMenuBar();
    public bool BeginMenu(string title, bool open = true) => ImGuiNET.ImGui.BeginMenu(title, open);
    public void EndMenu() => ImGuiNET.ImGui.EndMenu();

    // Window
    public bool BeginWindow(string title, bool open = true) => ImGuiNET.ImGui.Begin(title, ref open);
    public void EndWindow() => ImGuiNET.ImGui.End();
    public bool IsWindowHovered() => ImGuiNET.ImGui.IsWindowHovered();
    public bool IsWindowFocused() =>  ImGuiNET.ImGui.IsWindowFocused();
    public void SetWindowFocus() => ImGuiNET.ImGui.SetWindowFocus();

    public Vector2 GetContentRegionAvail() => new Vector2(ImGuiNET.ImGui.GetContentRegionAvail().X, ImGuiNET.ImGui.GetContentRegionAvail().Y);
    public Vector2 GetWindowPosition() => new Vector2(ImGuiNET.ImGui.GetWindowPos().X, ImGuiNET.ImGui.GetWindowPos().Y);
    public Vector2 GetCursorStartPos() => new Vector2(ImGuiNET.ImGui.GetCursorStartPos().X, ImGuiNET.ImGui.GetCursorStartPos().Y);

    // Widget
    public void Text(string text) => ImGuiNET.ImGui.Text(text);
    public bool Selectable(string text) => ImGuiNET.ImGui.Selectable(text);
    public void BulletText(string text) => ImGuiNET.ImGui.BulletText(text);
    public bool Button(string label) => ImGuiNET.ImGui.Button(label);
    public void Image(ITexture2D texture, float width, float height) => ImGuiNET.ImGui.Image(m_renderer.BindTexture(texture), new System.Numerics.Vector2(width, height));
    public void Checkbox(string label, ref bool value) => ImGuiNET.ImGui.Checkbox(label, ref value);
    public void SliderFloat(string label, ref float value, float min, float max) => ImGuiNET.ImGui.SliderFloat(label, ref value, min, max);
    public void PushStyleVar(IImGuiContext.StyleVar var, float indent) => ImGuiNET.ImGui.PushStyleVar((ImGuiNET.ImGuiStyleVar)(int)var, indent);
    public void PopStyleVar() => ImGuiNET.ImGui.PopStyleVar();

    // Tree
    public bool TreeNode(string text, IImGuiContext.TreeNodeFlags flags = IImGuiContext.TreeNodeFlags.None) => ImGuiNET.ImGui.TreeNodeEx(text, (ImGuiNET.ImGuiTreeNodeFlags)(int)flags);
    
    public void TreePop() => ImGuiNET.ImGui.TreePop();
    
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