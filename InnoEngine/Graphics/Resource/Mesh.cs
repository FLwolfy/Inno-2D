namespace InnoEngine.Graphics.Resource;

public class Mesh
{
    private readonly List<VertexAttributeEntry> m_attributes = new();
    private readonly Dictionary<string, int> m_attributeIndex = new();

    private readonly List<MeshSegment> m_segments = new();
    private uint[] m_indices = [];

    public string name { get; }
    public MeshRenderState renderState { get; set; }

    public int vertexCount => m_attributes.Count == 0 ? 0 : m_attributes[0].data.Length;
    public int indexCount => m_indices.Length;
    public int segmentCount => m_segments.Count;

    public Mesh(string name)
    {
        this.name = name;
    }
    
    public readonly struct VertexAttributeEntry(string name, Type type, Array data)
    {
        public readonly string name = name;
        public readonly Type elementType = type;
        public readonly Array data = data;
    }

    public void SetAttribute<T>(string attributeName, T[] data) where T : unmanaged
    {
        if (m_attributeIndex.ContainsKey(attributeName))
        {
            int idx = m_attributeIndex[attributeName];
            m_attributes[idx] = new VertexAttributeEntry(attributeName, typeof(T), data);
        }
        else
        {
            m_attributes.Add(new VertexAttributeEntry(attributeName, typeof(T), data));
            m_attributeIndex[attributeName] = m_attributes.Count - 1;
        }
    }

    public T[] GetAttribute<T>(string attributeName) where T : unmanaged
    {
        int idx = m_attributeIndex[attributeName];
        return (T[])m_attributes[idx].data;
    }

    public IReadOnlyList<VertexAttributeEntry> GetAllAttributes() => m_attributes;

    public void SetIndices(uint[] indices) => m_indices = indices;
    public uint[] GetIndices() => m_indices;

    public void AddSegment(MeshSegment meshSegment) => m_segments.Add(meshSegment);
    public IReadOnlyList<MeshSegment> GetSegments() => m_segments;
}
