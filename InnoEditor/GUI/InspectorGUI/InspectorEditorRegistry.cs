using System.Reflection;

namespace InnoEditor.GUI.InspectorGUI;

public static class InspectorEditorRegistry
{
    private static readonly Dictionary<Type, IInspectorEditor> REGISTRY = new();

    static InspectorEditorRegistry()
    {
        var editorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IInspectorEditor).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var editorType in editorTypes)
        {
            Register(editorType);
        }
    }

    public static void Register(Type type)
    {
        if (type.IsAbstract || type.IsInterface) return;

        var attr = type.GetCustomAttribute<InspectorEditorGUIAttribute>();
        if (attr == null) return;

        if (Activator.CreateInstance(type) is IInspectorEditor editor)
        {
            REGISTRY[attr.targetType] = editor;
        }
    }
    
    public static bool TryGetEditor(Type type, out IInspectorEditor? editor)
        => REGISTRY.TryGetValue(type, out editor);
}
