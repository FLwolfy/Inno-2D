namespace InnoEngine.Graphics.RenderPass.Pass;

/// <summary>
/// Represents a rendering stage in the pipeline.
/// </summary>
internal interface IRenderPass
{
    /// <summary>
    /// This is the tag representing the render sequence of render passes inside the render pipeline.
    /// </summary>
    RenderPassTag tag { get; }
    
    /// <summary>
    /// This method is used for each concrete renderPass class to render details based on the given RenderContext.
    /// </summary>
    void Render(RenderContext ctx);
}