using InnoEditor.Core;
using InnoEditor.GUI.PropertyGUI;
using InnoEngine.ECS;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class InspectorPanel : EditorPanel
{
    public override string title => "Inspector";

    internal InspectorPanel() {}
    
    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        GameObject? selectedObject = EditorManager.selection.selectedObject;
        if (selectedObject == null) { return; }

        // TEST
        foreach(var prop in selectedObject.transform.GetSerializedProperties())
        {
            PropertyRendererRegistry.GetRenderer(prop.propertyType)?.Bind(prop.name, () => prop.GetValue(), val => prop.SetValue(val));
        }
    }
}