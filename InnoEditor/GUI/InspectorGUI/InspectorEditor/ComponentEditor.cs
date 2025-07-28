using InnoEditor.GUI.PropertyGUI;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Serialization;

namespace InnoEditor.GUI.InspectorGUI.InspectorEditor;

[InspectorEditorGUI(typeof(GameComponent))]
public class ComponentEditor : IInspectorEditor
{
    // Default Render Components' Serialized Properties
    public virtual void OnInspectorGUI(object target)
    {
        if (target is not GameComponent comp) { return; }
        
        string compName = comp.GetType().Name;
        Action onClose = comp is Transform ? () => { } : () => comp.gameObject.RemoveComponent(comp);
        
        if (EditorGUILayout.CollapsingHeader(compName, onClose))
        {
            var serializedProps = comp.GetSerializedProperties().Where(p => p.visibility != PropertyVisibility.Hide).ToList();
            if (serializedProps.Count == 0)
            {
                EditorGUILayout.Label("No editable properties.");
            }
            
            foreach (var prop in serializedProps)
            {
                if (PropertyRendererRegistry.TryGetRenderer(prop.propertyType, out var renderer))
                {
                    renderer!.Bind(prop.name, () => prop.GetValue(), val => prop.SetValue(val), prop.visibility == PropertyVisibility.Show);
                }
                else
                {
                    EditorGUILayout.Label($"No renderer for {prop.name} ({prop.propertyType.Name})");
                }
            }
        }
    }
}