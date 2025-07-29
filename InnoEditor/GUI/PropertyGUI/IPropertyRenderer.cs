namespace InnoEditor.GUI.PropertyGUI;

public interface IPropertyRenderer
{
    /// <summary>
    /// Binds the property renderer to a property with the specified name.
    /// This method is used to connect the property renderer to a property in the target object.
    /// </summary>
    void Bind(string name, Func<object?> getter, Action<object?> setter, bool enabled);
}