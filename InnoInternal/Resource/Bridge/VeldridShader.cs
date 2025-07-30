using InnoInternal.Resource.Impl;

using Veldrid;
using Veldrid.SPIRV;

namespace InnoInternal.Resource.Bridge;

internal class VeldridShader : IShader
{
    public string name { get; }
    public ShaderStage stage { get; }
    public Shader veldridHandle { get; }

    private VeldridShader(string name, ShaderStage stage, Shader shader)
    {
        this.name = name;
        this.stage = stage;
        veldridHandle = shader;
    }

    public static VeldridShader LoadFromFile(GraphicsDevice device, string? path)
    {
        if (string.IsNullOrEmpty(path))
            return CreateWhitePixel(device);

        if (!File.Exists(path))
            throw new FileNotFoundException($"Shader file not found: {path}");

        string sourceCode = File.ReadAllText(path);
        string fileName = Path.GetFileName(path);
        string entryPoint = "main";

        // 推断 shader stage（可根据文件名后缀）
        ShaderStages veldridStage = InferShaderStage(fileName);
        ShaderStage shaderStage = ConvertStageEnum(veldridStage);

        // 编译 GLSL -> SPIR-V
        SpirvCompilationResult compilationResult = SpirvCompilation.CompileGlslToSpirv(sourceCode, fileName, veldridStage, new GlslCompileOptions());
        Shader shaderHandle = device.ResourceFactory.CreateShader(new ShaderDescription(veldridStage, compilationResult.SpirvBytes, "main"));

        return new VeldridShader(fileName, shaderStage, shaderHandle);
    }

    private static VeldridShader CreateWhitePixel(GraphicsDevice device)
    {
        // 一个最简单的 fragment shader，直接输出白色
        string fragCode = @"
#version 450
layout(location = 0) out vec4 fsout_Color;
void main() {
    fsout_Color = vec4(1.0);
}";
        ShaderStages veldridStage = ShaderStages.Fragment;
        ShaderStage stage = ShaderStage.Fragment;
        
        SpirvCompilationResult compilationResult = SpirvCompilation.CompileGlslToSpirv(fragCode, "white.frag", veldridStage, new GlslCompileOptions());
        Shader shaderHandle = device.ResourceFactory.CreateShader(new ShaderDescription(veldridStage, compilationResult.SpirvBytes, "main"));

        return new VeldridShader("WhitePixel", stage, shaderHandle);
    }

    private static ShaderStages InferShaderStage(string fileName)
    {
        string ext = Path.GetExtension(fileName).ToLower();
        return ext switch
        {
            ".vert" => ShaderStages.Vertex,
            ".vs" => ShaderStages.Vertex,
            ".frag" => ShaderStages.Fragment,
            ".fs" => ShaderStages.Fragment,
            _ => throw new InvalidOperationException($"Cannot infer shader stage from file extension: {fileName}")
        };
    }

    private static ShaderStage ConvertStageEnum(ShaderStages stage) => stage switch
    {
        ShaderStages.Vertex => ShaderStage.Vertex,
        ShaderStages.Fragment => ShaderStage.Fragment,
        _ => throw new NotSupportedException($"Unsupported shader stage: {stage}")
    };

}