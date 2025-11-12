using InnoBase.Math;

namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class QuaternionPropertyRenderer : PropertyRenderer<Quaternion>
{
    protected override void Bind(string name, Func<Quaternion> getter, Action<Quaternion> setter, bool enabled)
    {
        Quaternion value = getter.Invoke();
        if (EditorGUILayout.QuaternionField(name, ref value, enabled))
        {
            setter.Invoke(value);
        }
    }
}
