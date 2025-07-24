namespace InnoEngine.Serialization;

public class SerializedProperty
{
    private readonly Func<object?> m_getter;
    private readonly Action<object?> m_setter;
    
    public string name { get; }
    public Type propertyType { get; }
    public PropertyVisibility visibility { get; }

    internal SerializedProperty(string name, Type propertyType, Func<object?> getter, Action<object?> setter,  PropertyVisibility visibility)
    {
        this.name = name;
        this.propertyType = propertyType;
        this.visibility = visibility;
        
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
