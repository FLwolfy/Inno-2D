using InnoEngine.Graphics;

namespace InnoEngine.Core.Layer;

public abstract class Layer(string name = "Layer")
{
    public string name { get; } = name;

    public virtual void OnUpdate() { }
    public virtual void OnRender(RenderContext ctx) { }
    public virtual void OnEvent(object e) { }
    public virtual void OnAttach() { }
    public virtual void OnDetach() { }
}