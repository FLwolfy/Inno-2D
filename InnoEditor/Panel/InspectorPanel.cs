using InnoEditor.Core;
using InnoEditor.GUI.InspectorGUI;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel
{
    public class InspectorPanel : EditorPanel
    {
        public override string title => "Inspector";

        internal InspectorPanel() {}

        internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
        {
            // TODO: Change this to any type that needs to show Inspector View.
            var selectedObject = EditorManager.selection.selectedObject;
            if (selectedObject == null) { return; }
            
            if (InspectorEditorRegistry.TryGetEditor(selectedObject.GetType(), out var editor))
            {
                editor!.OnInspectorGUI(selectedObject);
            }
        }


    }
}
