namespace InnoEngine.Internal.Render.Impl;

internal interface IRenderAPI
{
    IRenderContext context { get; }
    IRenderCommand command { get; }
    ISpriteBatch spriteBatch { get; }
    
    void Initialize(object data);
}