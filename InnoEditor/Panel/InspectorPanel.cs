using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class InspectorPanel : EditorPanel
{
    public override string title => "Inspector View";

    internal InspectorPanel() {}
    
    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        context.Text("Inspector View Placeholder");
    }
}