using Veldrid.SPIRV;

using Inno.Assets.Types;

namespace Inno.Assets.Loaders;

internal class ShaderAssetLoader : InnoAssetLoader<ShaderAsset>
{
    private const string C_SHADER_ASSET_POSTFIX = ".shader";
    
    protected override ShaderAsset? LoadRaw(string relativeSourcePath, Guid guid)
    {
        string absoluteSourcePath = Path.Combine(AssetManager.assetDirectory, relativeSourcePath);
        if (!File.Exists(absoluteSourcePath)) return null;

        string glsl = File.ReadAllText(absoluteSourcePath);
        Veldrid.ShaderStages stage = DetectShaderStage(absoluteSourcePath);

        var compileResult = SpirvCompilation.CompileGlslToSpirv(
            glsl,
            null,
            stage,
            new GlslCompileOptions(true)
        );

        byte[] spirv = compileResult.SpirvBytes;
        string absShaderPath = Path.Combine(AssetManager.libraryDirectory, relativeSourcePath + C_SHADER_ASSET_POSTFIX);

        Directory.CreateDirectory(Path.GetDirectoryName(absShaderPath)!);
        File.WriteAllBytes(absShaderPath, spirv);

        var asset = new ShaderAsset(
            guid,
            relativeSourcePath,
            relativeSourcePath + C_SHADER_ASSET_POSTFIX
        );

        return asset;
    }

    protected override void UnloadRaw(string relativePath)
    {
        string absShaderPath = Path.Combine(AssetManager.libraryDirectory, relativePath + C_SHADER_ASSET_POSTFIX);
        
        Console.WriteLine("Unloading " + absShaderPath);

        if (File.Exists(absShaderPath))
            File.Delete(absShaderPath);
    }

    private static Veldrid.ShaderStages DetectShaderStage(string path)
    {
        string ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".vert" => Veldrid.ShaderStages.Vertex,
            ".frag" => Veldrid.ShaderStages.Fragment,
            _ => throw new Exception("Unknown shader stage: " + ext)
        };
    }

}