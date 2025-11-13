namespace InnoEngine.Graphics.Resource;

public class MeshSegment
{
    public string name { get; }
    public int indexStart { get; } 
    public int indexCount { get; }
    public int materialIndex { get; }

    public MeshSegment(string name, int indexStart, int indexCount, int materialIndex)
    {
        this.name = name;
        this.indexStart = indexStart;
        this.indexCount = indexCount;
        this.materialIndex = materialIndex;
    }
}