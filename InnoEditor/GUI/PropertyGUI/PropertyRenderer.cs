namespace InnoEditor.GUI.PropertyGUI;

public abstract class PropertyRenderer<T> : IPropertyRenderer
{
    protected abstract void Bind(string name, Func<T?> getter, Action<T?> setter);

    void IPropertyRenderer.Bind(string name, Func<object?> getter, Action<object?> setter)
    {
        Bind(name, () => (T?)getter.Invoke(), val => setter.Invoke(val));
    }
}