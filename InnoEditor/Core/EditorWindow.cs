using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Core;

public abstract class EditorWindow
{
    public bool IsOpen { get; set; } = true;

    public virtual void OnOpen() { }
    public virtual void OnClose() { }
    
    public abstract string Title { get; }
    internal abstract void OnGUI(IImGuiContext context, IRenderAPI renderAPI);
}
