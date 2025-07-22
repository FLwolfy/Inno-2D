using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Core;

public abstract class EditorPanel
{
    public bool isOpen { get; set; } = true;

    public virtual void OnOpen() { }
    public virtual void OnClose() { }
    
    public abstract string title { get; }
    internal abstract void OnGUI(IImGuiContext context, IRenderAPI renderAPI);
}
