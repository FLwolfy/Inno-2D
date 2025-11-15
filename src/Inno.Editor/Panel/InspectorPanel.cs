using Inno.Editor.Core;
using Inno.Editor.GUI.InspectorGUI;
using Inno.Graphics;
using Inno.Platform.ImGui;

namespace Inno.Editor.Panel
{
    public class InspectorPanel : EditorPanel
    {
        public override string title => "Inspector";

        internal InspectorPanel() {}

        internal override void OnGUI(IImGuiContext imGuiContext, RenderContext renderContext)
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
