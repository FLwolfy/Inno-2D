namespace Inno.Assets.Types;

public class ShaderAsset : InnoAsset
{
    public string shaderBinaryPath { get; internal set; } = null!;
    
    public ShaderAsset() {}
    internal ShaderAsset(Guid guid, string sourcePath, string shaderBinaryPath)
        : base(guid, sourcePath)
    {
        this.shaderBinaryPath = shaderBinaryPath;
    }
}