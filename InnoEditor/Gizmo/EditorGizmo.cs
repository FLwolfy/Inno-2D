using InnoInternal.ImGui.Impl;

namespace InnoEditor.Gizmo;

public abstract class EditorGizmo
{
    public bool isVisible = true;
    
    internal abstract void Draw(IImGuiContext context);
}