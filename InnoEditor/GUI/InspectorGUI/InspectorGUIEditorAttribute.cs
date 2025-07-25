namespace InnoEditor.GUI.InspectorGUI;

[AttributeUsage(AttributeTargets.Class)]
public class InspectorEditorGUIAttribute : Attribute
{
    public Type targetType;
    public InspectorEditorGUIAttribute(Type type) { targetType = type; }
}
