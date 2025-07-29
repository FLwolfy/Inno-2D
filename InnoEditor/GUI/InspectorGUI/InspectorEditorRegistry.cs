using System.Reflection;
using InnoEngine.Utility;

namespace InnoEditor.GUI.InspectorGUI;

public static class InspectorEditorRegistry
{
    private static readonly Dictionary<Type, IInspectorEditor> REGISTRY = new();

    internal static void Initialize()
    {
        TypeCacheManager.OnRefreshed += () =>
        {
            REGISTRY.Clear();
            
            foreach (var editorType in TypeCacheManager.GetTypesImplementing<IInspectorEditor>())
            {
                Register(editorType);
            }
        };
    }

    private static void Register(Type type)
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
