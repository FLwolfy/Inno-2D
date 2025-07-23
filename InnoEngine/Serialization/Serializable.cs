using System.Reflection;

namespace InnoEngine.Serialization;

public abstract class Serializable
{
    public List<SerializedProperty> GetSerializedProperties()
    {
        var result = new List<SerializedProperty>();
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.IsDefined(typeof(SerializablePropertyAttribute), true));

        foreach (var property in properties)
        {
            result.Add(new SerializedProperty
            (
                property.Name, 
                TypeDescriptor.Create(property.PropertyType), 
                () => property.GetValue(this),
                (val) => property.SetValue(this, val)
            ));
        }

        return result;
    }

}