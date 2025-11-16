namespace Inno.Editor.GUI.PropertyGUI;

public abstract class PropertyRenderer<T> : IPropertyRenderer
{
    /// <summary>
    /// Method to bind the property renderer to a property with the specified Type T.
    /// </summary>
    protected abstract void Bind(string name, Func<T?> getter, Action<T?> setter, bool enabled);

    void IPropertyRenderer.Bind(string name, Func<object?> getter, Action<object?> setter, bool enabled)
    {
        Bind(name, () => (T?)getter.Invoke(), val => setter.Invoke(val), enabled);
    }
}