using InnoInternal.Render.Impl;

namespace InnoEngine.Graphics.RenderObject;

public class Material : IDisposable
{
    public IShader vertexShader { get; }
    public IShader fragmentShader { get; }
    public IPipelineState pipeline { get; }
    public Dictionary<string, IUniformBuffer> uniformBuffers { get; }

    public Material(IShader vertexShader, IShader fragmentShader, IPipelineState pipeline, IUniformBuffer[] uniformBuffers)
    {
        this.vertexShader = vertexShader;
        this.fragmentShader = fragmentShader;
        this.pipeline = pipeline;
        
        this.uniformBuffers = new Dictionary<string, IUniformBuffer>();
        foreach (var ub in uniformBuffers)
        {
            this.uniformBuffers[ub.bufferName] = ub;
        }
    }

    public void Dispose()
    {
        vertexShader.Dispose();
        fragmentShader.Dispose();
        pipeline.Dispose();
        
        foreach (var ub in uniformBuffers.Values)
        {
            ub.Dispose();
        }
    }
}