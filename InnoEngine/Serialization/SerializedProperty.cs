namespace InnoEngine.Serialization;

public class SerializedProperty
{
    private readonly string m_name;
    private readonly TypeDescriptor m_typeDescriptor;
    private readonly Func<object?> m_getter;
    private readonly Action<object?> m_setter;
    
    public string name => m_name;
    public TypeDescriptor typeDescriptor => m_typeDescriptor;

    internal SerializedProperty(string name, TypeDescriptor typedObject, Func<object?> getter, Action<object?> setter)
    {
        m_name = name;
        m_typeDescriptor = typedObject;
        m_getter = getter;
        m_setter = setter;
    }
    
    public object? GetValue()
    {
        return m_getter.Invoke();
    }

    public void SetValue(object? value)
    {
        if (value == null)
        {
            if (m_typeDescriptor.isValueType && !m_typeDescriptor.isNullable)
                throw new ArgumentException($"Cannot assign null to non-nullable field {m_name} of type {m_typeDescriptor}.");
        }
        else
        {
            if (!m_typeDescriptor.IsAssignableFrom(value.GetType()))
                throw new ArgumentException($"Cannot assign value of type {value.GetType()} to field {m_name} of type {m_typeDescriptor}.");
        }

        m_setter.Invoke(value);
    }
}
