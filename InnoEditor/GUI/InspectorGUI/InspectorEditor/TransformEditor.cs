using InnoEditor.GUI.PropertyGUI;
using InnoEngine.ECS.Component;
using InnoEngine.Serialization;

namespace InnoEditor.GUI.InspectorGUI.InspectorEditor;

[InspectorEditorGUI(typeof(Transform))]
public class TransformEditor : ComponentEditor
{
    public override void OnInspectorGUI(object target)
    {
        if (target is not Transform comp) { return; }
        
        string compName = comp.GetType().Name;
        
        if (EditorGUILayout.CollapsingHeader(compName))
        {
            var serializedProps = comp.GetSerializedProperties().Where(p => p.visibility != SerializedProperty.PropertyVisibility.Hide).ToList();
            if (serializedProps.Count == 0)
            {
                EditorGUILayout.Label("No editable properties.");
            }
            
            foreach (var prop in serializedProps)
            {
                if (PropertyRendererRegistry.TryGetRenderer(prop.propertyType, out var renderer))
                {
                    renderer!.Bind(prop.name, () => prop.GetValue(), val => prop.SetValue(val), prop.visibility == SerializedProperty.PropertyVisibility.Show);
                }
                else
                {
                    EditorGUILayout.Label($"No renderer for {prop.name} ({prop.propertyType.Name})");
                }
            }
        }
    }
}