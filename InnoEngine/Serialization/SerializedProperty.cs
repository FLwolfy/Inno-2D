namespace InnoEngine.Serialization;

public class SerializedProperty
{
    private readonly Func<object?> m_getter;
    private readonly Action<object?> m_setter;
    
    public string name { get; }
    public Type type { get; }

    internal SerializedProperty(string name, Type typedObject, Func<object?> getter, Action<object?> setter)
    {
        this.name = name;
        type = typedObject;
        m_getter = getter;
        m_setter = setter;
    }
    
    public object? GetValue()
    {
        return m_getter.Invoke();
    }

    public void SetValue(object? value)
    {
        m_setter.Invoke(value);
    }
}
