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

    public bool BeginMainMenuBar() => ImGuiNET.ImGui.BeginMainMenuBar();
    public void EndMainMenuBar() => ImGuiNET.ImGui.EndMainMenuBar();

    public bool BeginWindow(string title, bool open = true) => ImGuiNET.ImGui.Begin(title, ref open);
    public void EndWindow() => ImGuiNET.ImGui.End();

    public bool BeginMenu(string title, bool open = true) => ImGuiNET.ImGui.BeginMenu(title, open);
    public void EndMenu() => ImGuiNET.ImGui.EndMenu();

    public Vector2 GetContentRegionAvail() => new Vector2(ImGuiNET.ImGui.GetContentRegionAvail().X, ImGuiNET.ImGui.GetContentRegionAvail().Y);

    public void Text(string text) => ImGuiNET.ImGui.Text(text);
    public bool Button(string label) => ImGuiNET.ImGui.Button(label);
    public void Image(ITexture2D texture, float width, float height) => ImGuiNET.ImGui.Image(m_renderer.BindTexture(texture), new System.Numerics.Vector2(width, height));
    public void Checkbox(string label, ref bool value) => ImGuiNET.ImGui.Checkbox(label, ref value);
    public void SliderFloat(string label, ref float value, float min, float max) => ImGuiNET.ImGui.SliderFloat(label, ref value, min, max);

    
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
    
    
    public bool IsWindowHovered() => ImGuiNET.ImGui.IsWindowHovered();
    public Vector2 GetWindowPosition() => new Vector2(ImGuiNET.ImGui.GetWindowPos().X, ImGuiNET.ImGui.GetWindowPos().Y);
    public Vector2 GetCursorStartPos() => new Vector2(ImGuiNET.ImGui.GetCursorStartPos().X, ImGuiNET.ImGui.GetCursorStartPos().Y);

    public bool IsMouseDown(int button) => ImGuiNET.ImGui.GetIO().MouseDown[button];
    public Vector2 GetMouseDelta() => new Vector2(ImGuiNET.ImGui.GetIO().MouseDelta.X, ImGuiNET.ImGui.GetIO().MouseDelta.Y);
    public Vector2 GetMousePosition() => new Vector2(ImGuiNET.ImGui.GetIO().MousePos.X, ImGuiNET.ImGui.GetIO().MousePos.Y);
    public float GetMouseWheel() => ImGuiNET.ImGui.GetIO().MouseWheel;
}