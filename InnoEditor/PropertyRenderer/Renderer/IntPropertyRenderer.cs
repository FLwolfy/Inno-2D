using InnoEditor.GUI;

namespace InnoEditor.PropertyRenderer.Renderer;

public class IntPropertyRenderer : PropertyRenderer<int>
{
    protected override void Render(int value)
    {
        EditorGUILayout.IntField("Int Value", ref value);
    }
}