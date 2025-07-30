namespace InnoInternal.Render.Impl;

internal interface IRenderAPI
{
    IRenderContext context { get; }
    IRenderCommand command { get; }
    IRenderer2D renderer2D { get; }
    
    void Initialize(object graphicDevice);
}