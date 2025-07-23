namespace InnoEditor.GUI.PropertyGUI;

public abstract class PropertyRenderer<T> : IPropertyRenderer
{
    protected abstract void Render(T value);

    void IPropertyRenderer.Render(object? value)
    {
        if (value is T tValue) {Render(tValue);}
        else {throw new ArgumentException($"Invalid type: expected {typeof(T)}, got {value?.GetType()}");}
    }
}