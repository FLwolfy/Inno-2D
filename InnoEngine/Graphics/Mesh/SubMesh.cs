namespace InnoEngine.Graphics.Mesh;

public class SubMesh
{
    public string name { get; }
    public int indexStart { get; } 
    public int indexCount { get; }
    public int materialIndex { get; }

    public SubMesh(string name, int indexStart, int indexCount, int materialIndex)
    {
        this.name = name;
        this.indexStart = indexStart;
        this.indexCount = indexCount;
        this.materialIndex = materialIndex;
    }
}