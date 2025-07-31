using System.Reflection;
using InnoBase;

namespace InnoInternal.Render.Impl;

internal interface IVertexDescription
{
    uint sizeInByte { get; }
    
    public static VertexElement[] CombineElements<T>() where T : IVertexDescription
    {
        var layout = new List<VertexElement>();
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<VertexElementAttribute>();
            if (attr != null)
            {
                layout.Add(new VertexElement(attr.semanticName, field.FieldType));
            }
        }

        return layout.ToArray();
    }
}


[AttributeUsage(AttributeTargets.Field)]
internal class VertexElementAttribute(string semanticName) : Attribute
{
    public string semanticName { get; } = semanticName;
}

internal struct VertexElement(string semanticName, Type fieldType)
{
    public readonly string semanticName = semanticName;
    public readonly Type fieldType = fieldType;
}



internal struct VertexPositionColor(Vector2 position, Color color) : IVertexDescription
{
    [VertexElement("Position")]
    public Vector2 position = position;

    [VertexElement("Color")]
    public Color color = color;

    public uint sizeInByte => 24;
}

internal struct VertexPositionTexture(Vector2 position, Vector2 texCoord) : IVertexDescription
{
    [VertexElement("Position")]
    public Vector2 position = position;

    [VertexElement("TexCoord")]
    public Vector2 texCoord = texCoord;

    public uint sizeInByte => 16;
}

internal struct VertexPositionColorTexture(Vector2 position, Color color, Vector2 texCoord) : IVertexDescription
{
    [VertexElement("Position")]
    public Vector2 position = position;

    [VertexElement("Color")]
    public Color color = color;
    
    [VertexElement("TexCoord")]
    public Vector2 texCoord = texCoord;

    public uint sizeInByte => 32;
}