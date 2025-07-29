using InnoEngine.ECS;

namespace InnoEditor.GUI.InspectorGUI.InspectorEditor;

[InspectorEditorGUI(typeof(GameObject))]
public class GameObjectEditor : IInspectorEditor
{
    public void OnInspectorGUI(object target)
    {
        if (target is not GameObject gameObject) { return; }
        
        // Render Components
        EditorGUILayout.BeginScope(gameObject.id.GetHashCode());
        OnShowComponents(gameObject);
        OnShowAddComponent(gameObject);
        EditorGUILayout.EndScope();
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
    }

    private void OnShowAddComponent(GameObject gameObject)
    {
        var existingTypes = gameObject.GetAllComponents()
            .Select(c => c.GetType())
            .ToHashSet();
        var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(GameComponent)) && !t.IsAbstract && !existingTypes.Contains(t))
            .ToList();
        
        var typeNames = componentTypes.Select(t => t.Name).ToArray();

        EditorGUILayout.BeginAlignment(EditorGUILayout.LayoutAlign.Center);

        if (EditorGUILayout.PopupMenu("Add Component", "No available components to add.", typeNames, out var selectedIndex))
        {
            var selectedType = componentTypes[selectedIndex!.Value];
            gameObject.AddComponent(selectedType);
        }

        EditorGUILayout.EndAlignment();
    }
}