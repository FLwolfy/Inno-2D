namespace InnoEngine.Serialization;

public class TypeDescriptor
{
    public Type type { get; private init; }
    public bool isGeneric => type.IsGenericType;
    public bool isArray => type.IsArray;
    public bool isPrimitive => type.IsPrimitive || type == typeof(string) || type.IsEnum;
    public bool isValueType => type.IsValueType;
    public bool isNullable => isValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    public TypeDescriptor? elementType { get; set; }
    public List<TypeDescriptor> genericArguments { get; set; } = new();
    

    public static TypeDescriptor Create(Type type)
    {
        var desc = new TypeDescriptor { type = type };

        if (type.IsArray)
        {
            desc.elementType = Create(type.GetElementType()!);
        }
        else if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
            {
                desc.genericArguments.Add(Create(arg));
            }
        }

        return desc;
    }

    public bool IsAssignableFrom(Type? other)
    {
        if (other == null)
            return false;

        if (type == other || type.IsAssignableFrom(other))
            return true;

        if (isArray && other.IsArray)
        {
            return elementType != null && elementType.IsAssignableFrom(other.GetElementType()!);
        }

        if (isGeneric && other.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() != other.GetGenericTypeDefinition())
                return false;

            var otherArgs = other.GetGenericArguments();
            if (genericArguments.Count != otherArgs.Length)
                return false;

            for (int i = 0; i < genericArguments.Count; i++)
            {
                if (!genericArguments[i].IsAssignableFrom(otherArgs[i]))
                    return false;
            }
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        if (isArray)
            return $"{elementType}[]";
        if (isGeneric)
            return $"{type.Name.Split('`')[0]}<{string.Join(", ", genericArguments)}>";
        return type.Name;
    }
}

