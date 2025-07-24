namespace InnoEditor.GUI.PropertyGUI;

public interface IPropertyRenderer
{
    void Bind(string name, Func<object?> getter, Action<object?> setter);
}