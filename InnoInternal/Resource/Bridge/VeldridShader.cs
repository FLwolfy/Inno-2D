using System.Text;

using InnoBase;
using InnoInternal.Render.Bridge;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

using Veldrid;

namespace InnoInternal.Resource.Bridge;

internal class VeldridShader : IShader
{
    private const string C_SHADER_ENTRY_POINT = "main";

    public ShaderState shaderState { get; }
    public ShaderDescription shaderDesc { get; }

    private VeldridShader(string name, ShaderDescription shaderDesc, ShaderState shaderState)
    {
        this.shaderState = shaderState;
        this.shaderDesc = shaderDesc;
    }

    public static VeldridShader LoadFromFile(IRenderCommand command, string? path)
    {
        if (string.IsNullOrEmpty(path))
            return CreateWhitePixel();

        if (!File.Exists(path))
            throw new FileNotFoundException($"Shader file not found: {path}");

        string sourceCode = File.ReadAllText(path);
        string fileName = Path.GetFileName(path);
        ShaderStages veldridStage = InferShaderStage(fileName);
        ShaderDescription shaderDesc = new ShaderDescription(veldridStage, Encoding.UTF8.GetBytes(sourceCode), C_SHADER_ENTRY_POINT);
        
        return new VeldridShader(fileName, shaderDesc, GetStateFromStage(veldridStage));
    }

    private static VeldridShader CreateWhitePixel()
    {
        // A Simple White frag code.
        string sourceCode = @"
#version 450
layout(location = 0) out vec4 fsout_Color;
void main() {
    fsout_Color = vec4(1.0);
}";
        string fileName = "WhitePixel";
        ShaderStages veldridStage = ShaderStages.Fragment;
        ShaderDescription shaderDesc = new ShaderDescription(veldridStage, Encoding.UTF8.GetBytes(sourceCode), C_SHADER_ENTRY_POINT);
        
        return new VeldridShader(fileName, shaderDesc, GetStateFromStage(veldridStage));
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

    private static ShaderState GetStateFromStage(ShaderStages stage)
    {
        return stage switch
        {
            ShaderStages.Vertex => ShaderState.Vertex,
            ShaderStages.Fragment => ShaderState.Fragment,
            ShaderStages.Compute => ShaderState.Compute,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), $"Unsupported shader stage: {stage}")
        };
    }
    
    public static VertexElementFormat GetFormatFromType(Type type)
    {
        return type switch
        {
            _ when type == typeof(Vector2) => VertexElementFormat.Float2,
            _ when type == typeof(Vector3) => VertexElementFormat.Float3,
            _ when type == typeof(Vector4) => VertexElementFormat.Float4,
            _ when type == typeof(Color)   => VertexElementFormat.Float4,
            _ when type == typeof(uint)    => VertexElementFormat.UInt1,
            _ => throw new NotSupportedException($"Unsupported vertex element type: {type}")
        };
    }

    public void SetUniform<T>(string name, T value)
    {
        throw new NotImplementedException();
    }

    public void SetTexture(string name, ITexture2D texture)
    {
        throw new NotImplementedException();
    }

    public void Bind()
    {
        throw new NotImplementedException();
    }
}