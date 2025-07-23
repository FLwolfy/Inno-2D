namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class IntPropertyRenderer : PropertyRenderer<int>
{
    protected override void Render(int value)
    {
        EditorGUILayout.IntField("Int Value", ref value);
    }
}