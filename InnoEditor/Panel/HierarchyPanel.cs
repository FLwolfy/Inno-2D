using InnoEditor.Core;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class HierarchyPanel : EditorPanel
{
    public override string title => "Hierarchy Panel";
    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        context.Text("Hierarchy View Placeholder");
    }
}