using InnoEngine.ECS;

namespace InnoEditor.GUI.InspectorGUI.InspectorEditor;

[InspectorEditorGUI(typeof(GameObject))]
public class GameObjectEditor : IInspectorEditor
{
    public void OnInspectorGUI(object target)
    {
        if (target is not GameObject gameObject) { return; }
        
        // Render Components
        OnShowComponents(gameObject);
    }

    private void OnShowComponents(GameObject gameObject)
    {
        var components = gameObject.GetAllComponents();
        foreach (var comp in components)
        {
            if (InspectorEditorRegistry.TryGetEditor(comp.GetType(), out var editor))
            {
                editor!.OnInspectorGUI(comp);
            }
            else if (InspectorEditorRegistry.TryGetEditor(typeof(GameComponent), out var defaultEditor))
            {
                defaultEditor!.OnInspectorGUI(comp);
            }
            
            EditorGUILayout.Space(10f);
        }
        
        EditorGUILayout.BeginAlignment(EditorGUILayout.LayoutAlign.Center);
        if (EditorGUILayout.Button("Add Component"))
        {
            Console.WriteLine("SYFCJS");
        }
        EditorGUILayout.EndAlignment();
    }
}