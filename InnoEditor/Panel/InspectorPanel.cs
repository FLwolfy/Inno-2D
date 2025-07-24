using InnoEditor.Core;
using InnoEditor.GUI;
using InnoEditor.GUI.PropertyGUI;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Serialization;
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
            GameObject? selectedObject = EditorManager.selection.selectedObject;
            if (selectedObject == null) { return; }
            context.PushID(selectedObject.id.GetHashCode());

            var components = selectedObject.GetAllComponents();
            foreach (var comp in components)
            {
                // Render Components
                string compName = comp.GetType().Name;
                bool openState = true;
                if (context.CollapsingHeader(compName, ref openState, IImGuiContext.TreeNodeFlags.DefaultOpen))
                {
                    var serializedProps = comp.GetSerializedProperties().Where(p => p.visibility != PropertyVisibility.Hide).ToList();;
                    if (serializedProps.Count == 0) { context.Text("No editable properties."); }
                    foreach (var prop in serializedProps)
                    {
                        var renderer = PropertyRendererRegistry.GetRenderer(prop.propertyType);
                        if (renderer == null)
                        {
                            context.Text($"No renderer for {prop.name} ({prop.propertyType.Name})");
                            continue;
                        }

                        renderer.Bind(prop.name, () => prop.GetValue(), val => prop.SetValue(val), prop.visibility == PropertyVisibility.Show);
                    }
                }
                
                // Check Delete
                if (!openState)
                {
                    if (comp is Transform) continue;
                    selectedObject.RemoveComponent(comp);
                }
            }
            
            context.PopID();
        }


    }
}
