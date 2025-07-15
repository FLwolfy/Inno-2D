using System.Numerics;
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
    
    public void BeginWindow(string title, bool open = true)
    {
        ImGuiNET.ImGui.Begin(title, ref open);
    }

    public void EndWindow()
    {
        ImGuiNET.ImGui.End();
    }

    public void Text(string text)
    {
        ImGuiNET.ImGui.Text(text);
    }

    public bool Button(string label)
    {
        return ImGuiNET.ImGui.Button(label);
    }

    public void Image(ITexture2D texture, float width, float height)
    {
        ImGuiNET.ImGui.Image(m_renderer.BindTexture(texture), new Vector2(width, height));
    }

    public void Checkbox(string label, ref bool value)
    {
        ImGuiNET.ImGui.Checkbox(label, ref value);
    }

    public void SliderFloat(string label, ref float value, float min, float max)
    {
        ImGuiNET.ImGui.SliderFloat(label, ref value, min, max);
    }

    public void DockSpace()
    {
        ImGuiNET.ImGui.DockSpace(ImGuiNET.ImGui.GetID("MyDockspace"));
    }

    public bool IsWindowHovered()
    {
        return ImGuiNET.ImGui.IsWindowHovered();
    }

    public float GetMouseWheel()
    {
        return ImGuiNET.ImGui.GetIO().MouseWheel;
    }
}