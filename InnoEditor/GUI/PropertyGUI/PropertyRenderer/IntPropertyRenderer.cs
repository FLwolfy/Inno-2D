namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class IntPropertyRenderer : PropertyRenderer<int>
{
    protected override void Bind(string name, Func<int> getter, Action<int> setter)
    {
        EditorGUILayout.Begin(LayoutType.Horizontal);
        
        int value = getter.Invoke();
        if (EditorGUILayout.IntField(name, ref value))
        {
            setter.Invoke(value);
        }
        
        EditorGUILayout.End();
    }
}