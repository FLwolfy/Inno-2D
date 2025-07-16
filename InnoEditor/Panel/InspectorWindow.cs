using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class InspectorWindow : EditorWindow
{
    public override string Title => "Inspector View";

    internal InspectorWindow() {}
    
    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        context.Text("Inspector View Placeholder");
    }
}