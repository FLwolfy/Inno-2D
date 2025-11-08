using InnoEngine.Graphics;
using InnoInternal.ImGui.Impl;

namespace InnoEditor.Core;

public abstract class EditorPanel
{
    public bool isOpen { get; set; } = true;

    public virtual void OnOpen() { }
    public virtual void OnClose() { }
    
    public abstract string title { get; }
    internal abstract void OnGUI(IImGuiContext imGuiContext, RenderContext renderContext);
}
