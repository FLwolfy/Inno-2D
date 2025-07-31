using InnoBase;
using InnoInternal.Render.Impl;
using InnoInternal.Resource.Impl;

namespace InnoInternal.Render.Bridge;

internal class VeldridRenderer2D : IRenderer2D
{
    private IRenderCommand m_command = null!;

    public void Initialize(IRenderCommand command)
    {
        m_command = command;
    }

    public void BeginScene()
    {
        m_command.Begin();
    }

    public void EndScene()
    {
        m_command.End();
    }

    public void Clear(Color color)
    {
        m_command.Clear(color);
    }

    public void DrawQuad(Matrix transform, IMaterial2D material, ITexture2D? textureOverride = null)
    {
        // TODO
    }
}
