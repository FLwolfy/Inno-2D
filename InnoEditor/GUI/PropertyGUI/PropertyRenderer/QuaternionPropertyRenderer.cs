using InnoBase;

namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class QuaternionPropertyRenderer : PropertyRenderer<Quaternion>
{
    protected override void Bind(string name, Func<Quaternion> getter, Action<Quaternion> setter)
    {
        // Use Vector3 for inspector showing
        Vector3 value = getter.Invoke().ToEulerAnglesXYZ();
        if (EditorGUILayout.Vector3Field(name, ref value))
        {
            setter.Invoke(Quaternion.FromEulerAnglesXYZ(value));
        }
    }
}