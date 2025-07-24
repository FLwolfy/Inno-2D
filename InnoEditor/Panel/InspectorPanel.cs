using System.Collections.Generic;
using InnoEditor.Core;
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

        private readonly Dictionary<GameComponent, bool> m_componentFoldouts = new();

        internal InspectorPanel() {}

        internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
        {
            GameObject? selectedObject = EditorManager.selection.selectedObject;
            if (selectedObject == null)
                return;

            var components = selectedObject.GetAllComponents();
            foreach (var comp in components)
            {
                string compName = comp.GetType().Name;
                bool openState = m_componentFoldouts.GetValueOrDefault(comp, true);
                
                // Check Delete
                if (!openState)
                {
                    if (comp is Transform) continue;
                    selectedObject.RemoveComponent(comp);
                    continue;
                }

                if (context.CollapsingHeader(compName, ref openState, IImGuiContext.TreeNodeFlags.DefaultOpen))
                {
                    m_componentFoldouts[comp] = openState;

                    var serializedProps = comp.GetSerializedProperties().Where(p => p.visibility != PropertyVisibility.Hide).ToList();;
                    if (serializedProps.Count == 0)
                    {
                        context.Text("No editable properties.");
                        continue;
                    }

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
                else
                {
                    m_componentFoldouts[comp] = openState;
                }
            }
        }


    }
}
